using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameEditor.Model
{
    [Serializable]
    public class GameData
    {        
        public string Name { get; set; }
        public string Description { get; set; }

        public GameData()
        {
            Name = string.Empty;
            Description = string.Empty;
        }

        public virtual GameData GetCopy()
        {
            return new GameData()
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
