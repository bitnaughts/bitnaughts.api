public static class SQL {
    public static class Format {
        /* CSV-like result output format */
        public const string DELIMITER = ",",
            NEW_LINE = "\n";

        /* How explicit/verbose telemetry receipts are (-1 == no restriction) */
        public const int MAX_CHARS_RETURED = 500;
        public const string VOIDED_CHARS = "\n\r\t";

        public const string SQL_FILES = "*.sql", SQL_FILE_TYPE = ".sql";
    }
    /* Common conditions */
    public const string COLUMNS = "columns",
        COLUMN = "column",
        VALUE = "value",
        ALL = "*";

    /* TABLE NAMES */
    public const string TABLE = "table";
        

    public const string CONDITION = "condition";

    public const string EQUALS = " = ";


    /* MS SQL has been shown to perform best when inserting groups of 25 values at a time. See https://www.red-gate.com/simple-talk/sql/performance/comparing-multiple-rows-insert-vs-single-row-insert-with-three-data-load-methods/ */
    public const int INSERT_BATCH_SIZE = 25;

    public static string IsEqual(string left, string right) {
        return left + EQUALS + right;
    }
}