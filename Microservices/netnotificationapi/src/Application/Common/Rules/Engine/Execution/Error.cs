namespace Application.Common.Rules.Engine.Execution
{
    public class Error
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public Error(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}
