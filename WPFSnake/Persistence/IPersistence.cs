using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFSnake.Persistence
{
    interface IPersistence
    {
        List<SaveEntry> List();
        Task<GameData> loadAsync(string path);
        Task saveAsync(string path, GameData data);
    }
}
