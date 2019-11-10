public static class HTTP {
    public const string GET = "get",
        POST = "post",
        DELETE = "delete",
        PUT = "put";
        
    public static class Endpoints {
        public const string GET = "get",
            SET = "set",
            UPDATE = "update",
            RESET = "reset",
            MINE = "mine";

        public static class Parameters {
            public const string FLAG = "flag",
                TYPE = "type",
                ID = "id",
                ASTEROID = "asteroid",
                SHIP = "ship",
                AMOUNT = "amount",
                DATE = "date";

            public static class Values {
                public const string RESET = "reset",
                    TYPE = "type",
                    ID = "id";
            }
        }

    }
}