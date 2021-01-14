namespace io.unlaunch.engine
{
    public enum Operator
    {
        EQ,     // equals
        NEQ,    // not equals
        GT,     // greater than
        GTE,    // greater than or equals
        LT,     // less than
        LTE,    // less than or equals
        SW,     // startsWith
        EW,     // endsWith
        CON,    // contains
        NCON,   // not contains
        NSW,    // not startsWith
        NEW,    // not endsWith
        PO,     // part of
        NPO,    // not part of
        HA,     // has any of
        NHA,    // does not has any of
        AO,     // has all of
        NAO     // does not have all of
    }
}
