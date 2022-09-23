
namespace Villedepommes
{

    using Row = IList<string>;
    interface Cursor
    {
        Row? next();
        void reset();
    };


    class Table : Cursor
    {
        protected int pos = 0;
        protected IList<Row>? rows;
        public Row? next()
        {

            if (rows == null)
            {
                throw new InvalidOperationException("rows are null!");
            }
            if (pos >= rows.Count)
            {
                return null;
            }

            return rows[pos++];
        }

        public void reset()
        {
            pos = 0;
        }
    };


    class TablePeople : Table
    {

        public TablePeople()
        {
            rows = new List<Row> {
            new List<string> {"1", "Nick"},
            new List<string> {"2", "Kelly"},
            new List<string> {"3", "Ivan"},
        };
        }

    };

    class TableAges : Table
    {
        public TableAges()
        {
            rows = new List<Row> {
            new List<string> {"1", "35"},
            new List<string> {"2", "33"},
            new List<string> {"3", "34"},
        };
        }

    };

    class TableLastNames : Table
    {
        public TableLastNames()
        {
            rows = new List<Row> {
            new List<string> {"1", "Korovaiko"},
            new List<string> {"2", "Peng"},
            new List<string> {"3", "Vassilenko"},
        };
        }

    };


    class Join : Cursor
    {

        public Join(Cursor c1, Cursor c2)
        {
            cursor1 = c1;
            cursor2 = c2;
            r1 = cursor1.next();
        }

        public void reset()
        {
            cursor1.reset();
            r1 = cursor1.next();
            cursor2.reset();
        }

        public Row? next()
        {
            while (r1 != null)
            {
                var r2 = cursor2.next();
                while (r2 != null)
                {
                    if (r1[0] == r2[0])
                    {
                        var result = new List<string>();
                        result.AddRange(r1);
                        result.AddRange(r2.Skip(1));
                        return result;
                    }
                    r2 = cursor2.next();
                }
                cursor2.reset();
                r1 = cursor1.next();
            }
            return null;
        }


        Row? r1;
        Cursor cursor1;
        Cursor cursor2;
    }


    class SqlExec
    {

        public static void Main(String[] args)
        {

            var tp = new TablePeople();
            var ta = new TableAges();
            var j = new Join(tp, ta);
            var tl = new TableLastNames();
            var j2 = new Join(tl, j);

            var r = j2.next();

            while (r != null)
            {
                Console.WriteLine(string.Join(", ", r));
                r = j2.next();
            }

        }
    };

}