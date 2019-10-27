public static class SQL {
    
    /* Common conditions */
    public const string COLUMNS = "columns",
        ALL = "*";

    /* TABLE NAMES */
    public const string TABLE = "table";
        

    public const string CONDITION = "condition";

    public const string EQUALS = " = ";


    /* MS SQL has been shown to perform best when inserting groups of 25 values at a time. See https://www.red-gate.com/simple-talk/sql/performance/comparing-multiple-rows-insert-vs-single-row-insert-with-three-data-load-methods/ */
    public const int INSERT_BATCH_SIZE = 25;
}