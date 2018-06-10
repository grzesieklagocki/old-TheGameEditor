using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace TheGameEditor.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class CharactersViewModel : ItemsViewModelBase<int>
    {
        public CharactersViewModel()
        {
        
        }
    }
}
