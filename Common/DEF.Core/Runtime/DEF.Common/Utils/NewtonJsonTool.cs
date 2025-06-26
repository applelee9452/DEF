namespace DEF
{
    public class NewtonJsonTool
    {
        public virtual string SerializeObject<T>(T obj) { return string.Empty; }

        public virtual T DeserializeObject<T>(string js_str) { return default; }
    }
}