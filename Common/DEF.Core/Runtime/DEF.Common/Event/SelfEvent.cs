namespace DEF
{
    public class SelfEvent
    {
        Entity Et { get; set; }

        public void SetEntity(Entity et)
        {
            Et = et;
        }

        public void Broadcast()
        {
            Et._HandleSelfEvent(this);
        }
    }
}