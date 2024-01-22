namespace YagirConsole.Scripts.Base.Shell
{
    public class Argument
    {
        private string argumentName;
        private ArgumentType type;

        private float numberValue = 0;
        private string stringValue = "";
        private bool logicValue = false;


        public bool LogicValue => logicValue;

        public string StringValue => stringValue;

        public float NumberValue => numberValue;

        public ArgumentType Type => type;
        public string ArgumentName => argumentName;

        public Argument(string argumentName, ArgumentType type)
        {
            this.argumentName = argumentName;
            this.type = type;
        }


        public Argument(string value)
        {
            if (float.TryParse(value, out numberValue))
            {
                type = ArgumentType.Number;
                return;
            }

            if (bool.TryParse(value, out logicValue))
            {
                type = ArgumentType.Bool;
                return;
            }

            type = ArgumentType.String;
            stringValue = value;
        }


        public static bool operator ==(Argument obj1, Argument obj2)
        {
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }
            return obj1.type == obj2.type;
        }

        public static bool operator !=(Argument obj1, Argument obj2)
        {
            return !(obj1 == obj2);
        }
            
        public override string ToString()
        {
            return $"Argument `{ArgumentName}`: " + this.type + " = " + 
                   (
                       (this.type == ArgumentType.Bool ? this.LogicValue.ToString() : 
                           (this.type == ArgumentType.Number ? this.numberValue.ToString() : this.stringValue))
                   );
        }

        public void SetName(string s)
        {
            argumentName = s;
        }
    }
}