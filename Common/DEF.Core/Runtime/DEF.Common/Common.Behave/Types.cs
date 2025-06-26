namespace NPBehave
{
    public enum Operator
    {
        IS_SET,
        IS_NOT_SET,
        IS_EQUAL,
        IS_NOT_EQUAL,
        IS_GREATER_OR_EQUAL,
        IS_GREATER,
        IS_SMALLER_OR_EQUAL,
        IS_SMALLER,
        ALWAYS_TRUE
    }

    public enum Stops
    {
        NONE,
        SELF,
        LOWER_PRIORITY,
        BOTH,
        IMMEDIATE_RESTART,
        LOWER_PRIORITY_IMMEDIATE_RESTART
    }
}