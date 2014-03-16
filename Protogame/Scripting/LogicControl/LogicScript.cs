using System.Collections.Generic;
using System.Linq;

namespace LogicControl
{
    public class LogicScript
    {
        public List<LogicStructure> Structures { get; set; }

        public List<LogicFunction> Functions { get; set; }

        public LogicScript(string code)
        {
            var parser = new LogicParser();
            parser.Parse(code);
            this.Structures = parser.Structures;
            this.Functions = parser.Functions;
        }

        public LogicUnmappedScriptInstance CreateUnmappedInstance()
        {
            return new LogicUnmappedScriptInstance(
                this.Structures.ToList(),
                this.Functions.ToList());
        }
    }

    public class LogicStructureInstance
    {
        public Dictionary<LogicField, object> Fields { get; set; }

        public Dictionary<LogicField, bool> FieldsSet { get; set; } 

        public LogicStructureInstance(LogicStructure structureDefinition)
        {
            this.Fields = structureDefinition.Fields.ToDictionary<LogicField, LogicField, object>(x => x, x => null);
            this.FieldsSet = structureDefinition.Fields.ToDictionary(x => x, x => false);
        }
    }
}
