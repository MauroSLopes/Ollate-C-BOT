using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula_Walker_Bot.database
{
    public class DBuser
    {
        public int ID { get; set; }
        public string discordID { get; set; }
        public int quantMensagem { get; set; }
        public ulong tempoCall { get; set; }
    }
}
