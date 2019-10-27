public static class Endpoints {
    public const string GET = "get",
        SET = "set",
        UPDATE = "update",
        RESET = "reset"; //DELETE = "delete", PUT = "put";

    public static class Parameters {
        public const string FLAG = "flag",
            TYPE = "type",
            ID = "id";
        public static class Values {
            public const string RESET = "reset",
                TYPE = "type",
                ID = "id";
        }
    }

}