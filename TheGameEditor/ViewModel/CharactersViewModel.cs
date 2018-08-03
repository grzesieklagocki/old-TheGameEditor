using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using TheGameEditor.Model;

namespace TheGameEditor.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class CharactersViewModel : ItemsViewModelBase<GameData>
    {
        public CharactersViewModel() : base(new List<GameData>(), new GameData() { Name = "Default GameData" })
        {
            
        }
    }
}
