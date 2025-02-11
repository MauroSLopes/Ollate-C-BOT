﻿using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Nebula_Walker_Bot
{
    public class JSONReader
    {

        public string token { get; set; }
        public string prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader SR = new StreamReader("config.json"))
            {
                string json = await SR.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);
                
                this.token = data.token;
                this.prefix = data.prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string prefix { get; set; }
    }
}
