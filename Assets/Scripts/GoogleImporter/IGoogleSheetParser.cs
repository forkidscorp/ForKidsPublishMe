using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleImporter
{
    public interface IGoogleSheetParser
    {
        public void Parse(string header, string token);
        
    }
}
