﻿namespace LiteDB.Studio
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal class TaskData
    {
        public static int RESULT_LIMIT { get; set; }
        public int Id { get; set; }
        public bool Executing { get; set; } = false;
        public string Filename { get; set; } = null;
        public string EditorContent { get; set; } = "";
        public string SelectedTab { get; set; } = "";
        public Tuple<int, int> Position { get; set; }
        public string Sql { get; set; } = "";
        public string Collection { get; set; } = "";
        public List<BsonValue> Result { get; set; } = null;
        public BsonDocument Parameters { get; set; } = new BsonDocument();

        public bool LimitExceeded { get; set; }
        public Exception Exception { get; set; } = null;
        public TimeSpan Elapsed { get; set; } = TimeSpan.Zero;

        public bool IsGridLoaded = false;
        public bool IsTextLoaded = false;
        public bool IsParametersLoaded = false;

        public Thread Thread { get; set; }
        public bool ThreadRunning { get; set; } = true;
        public ManualResetEventSlim WaitHandle { get; } = new ManualResetEventSlim(false);

        public void ReadResult(IBsonDataReader reader)
        {
            this.Result = new List<BsonValue>();
            this.LimitExceeded = false;
            this.Collection = reader.Collection;

            var index = 0;
            var hasLimit = this.Sql.IndexOf("LIMIT ", StringComparison.OrdinalIgnoreCase) >= 0;

            while (reader.Read())
            {
                if (index++ >= RESULT_LIMIT && hasLimit == false)
                {
                    this.LimitExceeded = true;
                    break;
                }

                this.Result.Add(reader.Current);
            }
        }
    }
}