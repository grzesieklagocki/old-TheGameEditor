using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.CSharp;
using PropertyChanged;
using System;
using System.CodeDom.Compiler;

namespace TheGameEditor.ViewModel
{
    public partial class ItemsViewModelBase<TItem>
    {
        [AddINotifyPropertyChangedInterface]
        public class UserProperty
        {
            public string CompilationErrorMessage { get; set; }
            public string SourceCode { get; set; } = defaultSourceCode;

            public Func<float> Calculate { get; set; }

            public RelayCommand CompileCommand
            {
                get
                {
                    return compileCommand ?? (compileCommand = new RelayCommand(CompileAction));
                }
            }

            public RelayCommand ResetCommand
            {
                get
                {
                    return resetCommand ?? (resetCommand = new RelayCommand(ResetAction));
                }
            }


            public bool HasErrors { get { return string.IsNullOrWhiteSpace(CompilationErrorMessage); } }


            private RelayCommand compileCommand;
            private RelayCommand resetCommand;

            private static readonly CSharpCodeProvider codeProvider;
            private static readonly CompilerParameters compilerParameters;
            private static readonly Func<float> defaultCalculateAction;
            private static readonly string defaultSourceCode;


            #region Static Constructor

            static UserProperty()
            {
                codeProvider = new CSharpCodeProvider();

                compilerParameters = new CompilerParameters
                {
                    IncludeDebugInformation = false,
                    GenerateExecutable = false,
                    GenerateInMemory = true                    
                };

                defaultCalculateAction = new Func<float>(() => 0f);

                defaultSourceCode = @"
                    using System;
            
                    namespace UserProperties
                    {                
                        public class UserProperty
                        {                
                            public static double Calculate()
                            {
                                return 0f;
                            }
                        }
                    }
                ";
            }

            #endregion


            private void CompileAction()
            {
                var results = codeProvider.CompileAssemblyFromSource(compilerParameters, SourceCode);

                CompilationErrorMessage = string.Empty;

                if (results.Errors.HasErrors)
                {
                    foreach (var error in results.Errors)
                    {
                        CompilationErrorMessage += error;
                    }

                    Calculate = defaultCalculateAction;
                }
                else
                {
                    var action = results.CompiledAssembly.GetType("UserProperties.UserProperty").GetMethod("Calculate");
                    Calculate = (Func<float>)Delegate.CreateDelegate(typeof(Func<float>), action);
                }
            }

            private void ResetAction()
            {
                SourceCode = defaultSourceCode;
                Calculate = defaultCalculateAction;
            }
        }
    }

}
