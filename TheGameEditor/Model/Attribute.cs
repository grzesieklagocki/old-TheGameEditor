using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameEditor.DataModels
{
    public class Attribute
    {
        /// <summary>
        /// Aktualna wartość atrybutu.
        /// </summary>
        public int Actual { get; set; }

        /// <summary>
        /// Nominalna wartość atrybutu
        /// </summary>
        public int Nominal { get; set; }

        public int Minimum { get; set; } = 0;

        public int Maximum { get; set; } = 100;

        /// <summary>
        /// Różnica między wartością aktualną a nominalną
        /// </summary>
        public int Difference => Actual - Nominal;

        /// <summary>
        /// Znormalizowana &lt;0, 1&gt; wartość atrybutu, (aktualna = nominalna) => (Normalized = 1).
        /// </summary>
        public float Normalized => Actual / Nominal;

        /// <summary>
        /// Procentowa &lt;0, 100&gt; wartość atrybutu, (aktualna = nominalna) => (Percentage = 100).
        /// </summary>
        public float Percentage => Normalized * 100f;


        public Attribute()
        {
            Minimum = 0;
            Maximum = 100;
        }


        public void AddValue(int value)
        {

        }
    }
}