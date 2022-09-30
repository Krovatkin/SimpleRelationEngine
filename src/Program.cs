
namespace Villedepommes
{

    using Row = IList<string>;
    public interface Cursor
    {
        Row? next();
        void reset();
    };


    public class Table : Cursor
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


    public class TablePeople : Table
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

    public class TableAges : Table
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

    public class TableLastNames : Table
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

    public interface Expression {
        dynamic eval(IDictionary<string, dynamic> st);
    }

    public class Param: Expression {

        public Param (string name) {
            this.name = name;
        }
        public virtual dynamic eval(IDictionary<string, dynamic> st) {
            return st[name];
        }

        protected string name;
    }

    public class BoolParam: Param {
        public BoolParam(string name): base(name) {}
        public override dynamic eval(IDictionary<string, dynamic> st) {
            return (bool)st[name];
            // var v = st[name];
            // if (v is bool) {
            //     return v;
            // } else {
            //     throw new InvalidCastException("Expected the value to be a bool");
            // }
        }
    }

    public abstract class BinaryExpression: Expression {

        public BinaryExpression(Expression left, Expression right) {
            this.left = left;
            this.right = right;
        }

        public abstract dynamic eval(IDictionary<string, dynamic> st);

        protected Expression left;
        protected Expression right;
    }

    public class And: BinaryExpression {

        public And(Expression left, Expression right): base(left, right) {}
        public override dynamic eval(IDictionary<string, dynamic> st) {
            return (bool) left.eval(st) && (bool) right.eval(st);
        }
    }

    public class Or: BinaryExpression {

        public Or(Expression left, Expression right): base(left, right) {}
        public override dynamic eval(IDictionary<string, dynamic> st) {
            return (bool) left.eval(st) || (bool) right.eval(st);
        }
    }

    public class Eq: BinaryExpression {

        public Eq(Expression left, Expression right): base(left, right) {}
        public override dynamic eval(IDictionary<string, dynamic> st) {
            return (bool) left.eval(st) == (bool) right.eval(st);
        }
    }

    public abstract class UnaryExpression: Expression {

        public UnaryExpression(Expression inner) {
            this.inner = inner;
        }

        public abstract dynamic eval(IDictionary<string, dynamic> st);

        protected Expression inner;
    }

    public class Not: UnaryExpression {

        public Not(Expression inner): base(inner) {}
        public override dynamic eval(IDictionary<string, dynamic> st) {
            return !(bool) inner.eval(st);
        }
    }

    public class Join : Cursor
    {

        static Expression defaultEquality = new Eq(new Param("left"), new Param("right"));

        public Join(Cursor c1, Cursor c2, Expression? expr = null, string? leftName = null, string? rightName = null)
        {
            this.expr = expr;
            this.leftName = leftName;
            this.rightName = rightName;
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
                    var st = new Dictionary<string, dynamic>();

                    st[leftName ?? "left"] = r1[0];
                    st[rightName ?? "right"] = r2[0];

                    if ((bool)defaultEquality.eval(st))
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

        Expression? expr;

        string? leftName;
        string? rightName;
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