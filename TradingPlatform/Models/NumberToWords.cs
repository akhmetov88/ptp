using System;
using System.Text;

namespace TradingPlatform.Models
{
    public class NumberToWords
    {
        /// <summary>
        /// Class для записи денежных сумм samplesю: "тысяча рублей 00 копеек".
        /// </summary>
        /// <example>
        /// Summ.ToWords (100, Currency.Rubles); // "сто рублей 00 копеек"
        /// Currency.Rubles.ToWords (123.45); // "сто двадцать три рубля 45 копеек"
        /// </example>
        public static class Summ
        {
            /// <summary>
            /// Записывает samples суммы в заданной валюте в <paramref name="result"/> строчными буквами.
            /// </summary>
            public static StringBuilder ToWords(decimal summ, Currency curr, StringBuilder result)
            {
                decimal whole = Math.Floor(summ);
                uint fractional = (uint)((summ - whole) * 100);

                Number.ToWords(whole, curr.MainFractional, result);
                return AddPennies(fractional, curr, result);
            }

            /// <summary>
            /// Записывает samples суммы в заданной валюте в <paramref name="result"/> строчными буквами.
            /// </summary>
            public static StringBuilder ToWords(double summ, Currency curr, StringBuilder result)
            {
                double whole = Math.Floor(summ);

                // Вынесение 100 за скобки позволяет избежать ошибки округления
                // например, когда summ = 1234.51.
                uint fractional = (uint)(summ * 100) - (uint)(whole * 100);

                Number.ToWords(whole, curr.MainFractional, result);
                return AddPennies(fractional, curr, result);
            }

            private static StringBuilder AddPennies(uint fractional, Currency curr, StringBuilder result)
            {
                result.Append(' ');

                // Эта строчка выполняется быстрее, чем следующая за ней закомментированная.
                result.Append(fractional.ToString("00"));
                //result.AppendFormat ("{0:00}", fractional);

                result.Append(' ');
                result.Append(Number.ToApprove(curr.FractionalUnit, fractional));

                return result;
            }

            /// <summary>
            /// Проверяет, подходит ли number для передачи методу 
            /// <see cref="Summ.ToWords (decimal, Currency)"/>.
            /// </summary>
            /// <remarks>
            /// Сумма должна быть неотрицательной и должна содержать 
            /// не более двух цифр после запятой.
            /// </remarks>
            /// <returns>
            /// Описание нарушенного ограничения или null.
            /// </returns>
            public static string CheckSumm(decimal summ)
            {
                if (summ < 0) return "Сумма должна быть неотрицательной.";

                decimal whole = Math.Floor(summ);
                decimal fractional = (summ - whole) * 100;

                if (Math.Floor(fractional) != fractional)
                {
                    return "Сумма должна содержать не более двух цифр после запятой.";
                }

                return null;
            }

            /// <summary>
            /// Возвращает samples заданной суммы строчными буквами.
            /// </summary>
            public static string ToWords(decimal n, Currency curr)
            {
                return Number.ApplyCaps(ToWords(n, curr, new StringBuilder()), Capital.No);
            }

            /// <summary>
            /// Возвращает samples заданной суммы.
            /// </summary>
            public static string ToWords(decimal n, Currency curr, Capital capital)
            {
                return Number.ApplyCaps(ToWords(n, curr, new StringBuilder()), capital);
            }

            /// <summary>
            /// Возвращает samples заданной суммы строчными буквами.
            /// </summary>
            public static string ToWords(double n, Currency curr)
            {
                return Number.ApplyCaps(ToWords(n, curr, new StringBuilder()), Capital.No);
            }

            /// <summary>
            /// Возвращает samples заданной суммы.
            /// </summary>
            public static string ToWords(double n, Currency curr, Capital capital)
            {
                return Number.ApplyCaps(ToWords(n, curr, new StringBuilder()), capital);
            }
        }

        /// <summary>
        /// Class для преобразования чисел в samples на русском языке.
        /// </summary>
        /// <example>
        /// Number.ToWords (1, GenitiveNumber.Male); // "один"
        /// Number.ToWords (2, GenitiveNumber.Female); // "две"
        /// Number.ToWords (21, GenitiveNumber.Neuter); // "двадцать одно"
        /// </example>
        /// <example>
        /// Number.ToWords (5, new Unit (
        ///  GenitiveNumber.Male, "метр", "метра", "метров"), sb); // "пять метров"
        /// </example>
        public static class Number
        {
            /// <summary>
            /// Получить samples числа с согласованной единицей измерения.
            /// </summary>
            /// <param name="number"> Число должно быть целым, неотрицательным. </param>
            /// <param name="еи"></param>
            /// <param name="result"> Сюда записывается результат. </param>
            /// <returns> <paramref name="result"/> </returns>
            /// <exception cref="ArgumentException">
            /// Если number меньше нуля или не целое. 
            /// </exception>
            public static StringBuilder ToWords(decimal number, IUnit u, StringBuilder result)
            {
                string error = CheckNumber(number);
                if (error != null) throw new ArgumentException(error, "number");

                // Целочисленные версии работают в разы быстрее, чем decimal.
                if (number <= uint.MaxValue)
                {
                    ToWords((uint)number, u, result);
                }
                else if (number <= ulong.MaxValue)
                {
                    ToWords((ulong)number, u, result);
                }
                else
                {
                    MyStringBuilder mySb = new MyStringBuilder(result);

                    decimal div1000 = Math.Floor(number / 1000);
                    ToWordsSeniorClasses(div1000, 0, mySb);
                    ToWordsClasses((uint)(number - div1000 * 1000), u, mySb);
                }

                return result;
            }

            /// <summary>
            /// Получить samples числа с согласованной единицей измерения.
            /// </summary>
            /// <param name="number"> 
            /// Число должно быть целым, неотрицательным, не большим <see cref="MaxDouble"/>. 
            /// </param>
            /// <param name="еи"></param>
            /// <param name="result"> Сюда записывается результат. </param>
            /// <exception cref="ArgumentException">
            /// Если number меньше нуля, не целое или больше <see cref="MaxDouble"/>. 
            /// </exception>
            /// <returns> <paramref name="result"/> </returns>
            /// <remarks>
            /// float по умолчанию преобразуется к double, поэтому нет перегрузки для float.
            /// В результате ошибок округления возможно расхождение цифр прописи и
            /// строки, выдаваемой double.ToString ("R"), начиная с 17 значащей цифры.
            /// </remarks>
            public static StringBuilder ToWords(double number, IUnit u, StringBuilder result)
            {
                string error = CheckNumber(number);
                if (error != null) throw new ArgumentException(error, "number");

                if (number <= uint.MaxValue)
                {
                    ToWords((uint)number, u, result);
                }
                else if (number <= ulong.MaxValue)
                {
                    // ToWords с ulong выполняется в среднем в 2 раза быстрее.
                    ToWords((ulong)number, u, result);
                }
                else
                {
                    MyStringBuilder mySb = new MyStringBuilder(result);

                    double div1000 = Math.Floor(number / 1000);
                    ToWordsSeniorClasses(div1000, 0, mySb);
                    ToWordsClasses((uint)(number - div1000 * 1000), u, mySb);
                }

                return result;
            }

            /// <summary>
            /// Получить samples числа с согласованной единицей измерения.
            /// </summary>
            /// <returns> <paramref name="result"/> </returns>
            public static StringBuilder ToWords(ulong number, IUnit u, StringBuilder result)
            {
                if (number <= uint.MaxValue)
                {
                    ToWords((uint)number, u, result);
                }
                else
                {
                    MyStringBuilder mySb = new MyStringBuilder(result);

                    ulong div1000 = number / 1000;
                    ToWordsSeniorClasses(div1000, 0, mySb);
                    ToWordsClasses((uint)(number - div1000 * 1000), u, mySb);
                }

                return result;
            }

            /// <summary>
            /// Получить samples числа с согласованной единицей измерения.
            /// </summary>
            /// <returns> <paramref name="result"/> </returns>
            public static StringBuilder ToWords(uint number, IUnit u, StringBuilder result)
            {
                MyStringBuilder mySb = new MyStringBuilder(result);

                if (number == 0)
                {
                    mySb.Append("ноль");
                    mySb.Append(u.GenitivePlural);
                }
                else
                {
                    uint div1000 = number / 1000;
                    ToWordsSeniorClasses(div1000, 0, mySb);
                    ToWordsClasses(number - div1000 * 1000, u, mySb);
                }

                return result;
            }

            /// <summary>
            /// Записывает в <paramref name="sb"/> samples числа, начиная с самого 
            /// старшего класса до класса с номером <paramref name="numberOfClasses"/>.
            /// </summary>
            /// <param name="sb"></param>
            /// <param name="number"></param>
            /// <param name="numberOfClasses">0 = класс тысяч, 1 = миллионов и т.д.</param>
            /// <remarks>
            /// В методе применена рекурсия, чтобы обеспечить запись в StringBuilder 
            /// в нужном порядке - от старших классов к младшим.
            /// </remarks>
            static void ToWordsSeniorClasses(decimal number, int numberOfClasses, MyStringBuilder sb)
            {
                if (number == 0) return; // конец рекурсии

                // Записать в StringBuilder samples старших классов.
                decimal div1000 = Math.Floor(number / 1000);
                ToWordsSeniorClasses(div1000, numberOfClasses + 1, sb);

                uint numberTo999 = (uint)(number - div1000 * 1000);
                if (numberTo999 == 0) return;

                ToWordsClasses(numberTo999, Classes[numberOfClasses], sb);
            }

            static void ToWordsSeniorClasses(double number, int numberOfClasses, MyStringBuilder sb)
            {
                if (number == 0) return; // конец рекурсии

                // Записать в StringBuilder samples старших классов.
                double div1000 = Math.Floor(number / 1000);
                ToWordsSeniorClasses(div1000, numberOfClasses + 1, sb);

                uint numberTo999 = (uint)(number - div1000 * 1000);
                if (numberTo999 == 0) return;

                ToWordsClasses(numberTo999, Classes[numberOfClasses], sb);
            }

            static void ToWordsSeniorClasses(ulong number, int numberOfClasses, MyStringBuilder sb)
            {
                if (number == 0) return; // конец рекурсии

                // Записать в StringBuilder samples старших классов.
                ulong div1000 = number / 1000;
                ToWordsSeniorClasses(div1000, numberOfClasses + 1, sb);

                uint numberTo999 = (uint)(number - div1000 * 1000);
                if (numberTo999 == 0) return;

                ToWordsClasses(numberTo999, Classes[numberOfClasses], sb);
            }

            static void ToWordsSeniorClasses(uint number, int numberOfClasses, MyStringBuilder sb)
            {
                if (number == 0) return; // конец рекурсии

                // Записать в StringBuilder samples старших классов.
                uint div1000 = number / 1000;
                ToWordsSeniorClasses(div1000, numberOfClasses + 1, sb);

                uint numberTo999 = number - div1000 * 1000;
                if (numberTo999 == 0) return;

                ToWordsClasses(numberTo999, Classes[numberOfClasses], sb);
            }

            #region ToWordsClasses

            /// <summary>
            /// Формирует запись класса с названием, например,
            /// "125 тысяч", "15 рублей".
            /// Для 0 записывает только единицу измерения в kind.мн.
            /// </summary>
            private static void ToWordsClasses(uint numberTo999, IUnit unit, MyStringBuilder sb)
            {
                uint numberUnits = numberTo999 % 10;
                uint numberDecades = (numberTo999 / 10) % 10;
                uint numberHundred = (numberTo999 / 100) % 10;

                sb.Append(Сотни[numberHundred]);

                if ((numberTo999 % 100) != 0)
                {
                    Decades[numberDecades].ToWords(sb, numberUnits, unit.GenitiveNumber);
                }

                // Добавить название класса в нужной форме.
                sb.Append(ToApprove(unit, numberTo999));
            }

            #endregion

            #region CheckNumber

            /// <summary>
            /// Проверяет, подходит ли number для передачи методу 
            /// <see cref="ToWords(decimal,IUnit,StringBuilder)"/>.
            /// </summary>
            /// <returns>
            /// Описание нарушенного ограничения или null.
            /// </returns>
            public static string CheckNumber(decimal number)
            {
                if (number < 0)
                    return "Число должно быть больше или равно нулю.";

                if (number != decimal.Floor(number))
                    return "Число не должно содержать дробной части.";

                return null;
            }

            /// <summary>
            /// Проверяет, подходит ли number для передачи методу 
            /// <see cref="ToWords(double,IUnit,StringBuilder)"/>.
            /// </summary>
            /// <returns>
            /// Описание нарушенного ограничения или null.
            /// </returns>
            public static string CheckNumber(double number)
            {
                if (number < 0)
                    return "Число должно быть больше или равно нулю.";

                if (number != Math.Floor(number))
                    return "Число не должно содержать дробной части.";

                if (number > MaxDouble)
                {
                    return "Число должно быть не больше " + MaxDouble + ".";
                }

                return null;
            }

            #endregion

            #region ToApprove

            /// <summary>
            /// ToApprove название единицы измерения с numberм.
            /// Например, согласование единицы (рубль, рубля, рублей) 
            /// с numberм 23 даёт "рубля", а с numberм 25 - "рублей".
            /// </summary>
            public static string ToApprove(IUnit unit, uint number)
            {
                uint numberUnits = number % 10;
                uint numberDecades = (number / 10) % 10;

                if (numberDecades == 1) return unit.GenitivePlural;
                switch (numberUnits)
                {
                    case 1: return unit.NominativeSingular;
                    case 2: case 3: case 4: return unit.GenitiveSingular;
                    default: return unit.GenitivePlural;
                }
            }

            /// <summary>
            /// ToApprove название единицы измерения с numberм.
            /// Например, согласование единицы (рубль, рубля, рублей) 
            /// с numberм 23 даёт "рубля", а с numberм 25 - "рублей".
            /// </summary>
            public static string ToApprove(IUnit unit, decimal number)
            {
                return ToApprove(unit, (uint)(number % 100));
            }

            #endregion

            #region Единицы

            static string ToWordsNumbers(uint number, GenitiveNumber kind)
            {
                return Numbers[number].ToWords(kind);
            }

            abstract class NumberClass
            {
                public abstract string ToWords(GenitiveNumber kind);
            }

            class NumberToChangest : NumberClass, IVariesByGender
            {
                public NumberToChangest(
                    string male,
                    string female,
                    string neuter,
                    string plural)
                {
                    this.male = male;
                    this.female = female;
                    this.neuter = neuter;
                    this.plural = plural;
                }

                public NumberToChangest(
                    string single,
                    string plural)

                    : this(single, single, single, plural)
                {
                }

                private readonly string male;
                private readonly string female;
                private readonly string neuter;
                private readonly string plural;

                #region IVariesByGender Members

                public string Male { get { return this.male; } }
                public string Female { get { return this.female; } }
                public string Neuter { get { return this.neuter; } }
                public string Plural { get { return this.plural; } }

                #endregion

                public override string ToWords(GenitiveNumber kind)
                {
                    return kind.GetForm(this);
                }
            }

            class NumberNoChangest : NumberClass
            {
                public NumberNoChangest(string samples)
                {
                    this.samples = samples;
                }

                private readonly string samples;

                public override string ToWords(GenitiveNumber kind)
                {
                    return this.samples;
                }
            }

            static readonly NumberClass[] Numbers = new NumberClass[]
            {
            null,
            new NumberToChangest ("один", "одна", "одне", "одні"),
            new NumberToChangest ("дві", "дві", "дві", "двоє"),
            new NumberToChangest ("три", "троє"),
            new NumberToChangest ("чотири", "четверо"),
            new NumberNoChangest ("п'ять"),
            new NumberNoChangest ("шість"),
            new NumberNoChangest ("сім"),
            new NumberNoChangest ("вісім"),
            new NumberNoChangest ("девять"),
            };

            #endregion
            #region Десятки

            static readonly Decade[] Decades = new Decade[]
            {
            new FirstDecade (),
            new SecondDecade (),
            new SimpleDecade ("двадцять"),
            new SimpleDecade ("тридцять"),
            new SimpleDecade ("сорок"),
            new SimpleDecade ("п'ятдесят"),
            new SimpleDecade ("шістдесят"),
            new SimpleDecade ("сімдесят"),
            new SimpleDecade ("вісімдесят"),
            new SimpleDecade ("дев'яносто")
            };

            abstract class Decade
            {
                public abstract void ToWords(MyStringBuilder sb, uint numberUnits, GenitiveNumber kind);
            }

            class FirstDecade : Decade
            {
                public override void ToWords(MyStringBuilder sb, uint numberUnits, GenitiveNumber kind)
                {
                    sb.Append(ToWordsNumbers(numberUnits, kind));
                }
            }

            class SecondDecade : Decade
            {
                static readonly string[] ToWordsOnDtsat = new string[]
                {
                "десять",
                "одинадцять",
                "дванадцять",
                "тринадцять",
                "чотирнадцять",
                "п'ятнадцять",
                "шістнадцять",
                "сімнадцять",
                "вісімнадцять",
                "дев'ятнадцять"
                };

                public override void ToWords(MyStringBuilder sb, uint numberUnits, GenitiveNumber kind)
                {
                    sb.Append(ToWordsOnDtsat[numberUnits]);
                }
            }

            class SimpleDecade : Decade
            {
                public SimpleDecade(string decadeName)
                {
                    this.decadeName = decadeName;
                }

                private readonly string decadeName;

                public override void ToWords(MyStringBuilder sb, uint numberUnits, GenitiveNumber kind)
                {
                    sb.Append(this.decadeName);

                    if (numberUnits == 0)
                    {
                        // После "двадцать", "тридцать" и т.д. не пишут "ноль" (единиц)
                    }
                    else
                    {
                        sb.Append(ToWordsNumbers(numberUnits, kind));
                    }
                }
            }

            #endregion
            #region Сотни

            static readonly string[] Сотни = new string[]
            {
            null,
            "сто",
            "двісті",
            "триста",
            "чотириста",
            "п'ятсот",
            "шістсот",
            "сімсот",
            "вісімсот",
            "дев'ятсот"
            };

            #endregion
            #region Classes

            #region ClassТысяч

            class ClassThousand : IUnit
            {
                public string NominativeSingular { get { return "тисяча"; } }
                public string GenitiveSingular { get { return "тисячі"; } }
                public string GenitivePlural { get { return "тисяч"; } }
                public GenitiveNumber GenitiveNumber { get { return GenitiveNumber.Female; } }
            }

            #endregion
            #region Class

            class Class : IUnit
            {
                readonly string initialForm; 

                public Class(string initialForm)
                {
                    this.initialForm = initialForm;
                }

                public string NominativeSingular { get { return this.initialForm; } }
                public string GenitiveSingular { get { return this.initialForm + "а"; } }
                public string GenitivePlural { get { return this.initialForm + "ів"; } }
                public GenitiveNumber GenitiveNumber { get { return GenitiveNumber.Male; } }
            }

            #endregion

            /// <summary>
            /// Class - группа из 3 цифр.  Есть классы единиц, тысяч, миллионов и т.д.
            /// </summary>
            static readonly IUnit[] Classes = new IUnit[]
            {
            new ClassThousand (),
            new Class ("мільйон"),
            new Class ("мільярд"),
            new Class ("трильйон"),
            new Class ("квадрильйон"),
            new Class ("квінтильйон"),
            new Class ("секстильйон"),
            new Class ("септілліон"),
            new Class ("октілліон"),

                // Это количество классов покрывает весь диапазон типа decimal.
            };

            #endregion

            #region MaxDouble

            /// <summary>
            /// Максимальное number типа double, представимое в виде прописи.
            /// </summary>
            /// <remarks>
            /// Рассчитывается исходя из количества определённых классов.
            /// Если добавить ещё классы, оно будет автоматически увеличено.
            /// </remarks>
            public static double MaxDouble
            {
                get
                {
                    if (maxDouble == 0)
                    {
                        maxDouble = CalcMaxDouble();
                    }

                    return maxDouble;
                }
            }

            private static double maxDouble = 0;

            static double CalcMaxDouble()
            {
                double max = Math.Pow(1000, Classes.Length + 1);

                double d = 1;

                while (max - d == max)
                {
                    d *= 2;
                }

                return max - d;
            }

            #endregion

            #region Вспомогательные классы

            #region Форма

            #endregion
            #region MyStringBuilder

            /// <summary>
            /// Вспомогательный класс, аналогичный <see cref="StringBuilder"/>.
            /// Между вызовами <see cref="MyStringBuilder.Append"/> вставляет пробелы.
            /// </summary>
            class MyStringBuilder
            {
                public MyStringBuilder(StringBuilder sb)
                {
                    this.sb = sb;
                }

                readonly StringBuilder sb;
                bool insertSpace = false;

                /// <summary>
                /// Добавляет word <paramref name="s"/>,
                /// вставляя перед ним пробел, если нужно.
                /// </summary>
                public void Append(string s)
                {
                    if (string.IsNullOrEmpty(s)) return;

                    if (this.insertSpace)
                    {
                        this.sb.Append(' ');
                    }
                    else
                    {
                        this.insertSpace = true;
                    }

                    this.sb.Append(s);
                }

                public override string ToString()
                {
                    return sb.ToString();
                }
            }

            #endregion

            #endregion

            #region Перегрузки метода ToWords, возвращающие string

            /// <summary>
            /// Возвращает samples числа строчными буквами.
            /// </summary>
            public static string ToWords(decimal number, IUnit u)
            {
                return ToWords(number, u, Capital.No);
            }

            /// <summary>
            /// Возвращает samples числа.
            /// </summary>
            public static string ToWords(decimal number, IUnit u, Capital capital)
            {
                return ApplyCaps(ToWords(number, u, new StringBuilder()), capital);
            }

            /// <summary>
            /// Возвращает samples числа строчными буквами.
            /// </summary>
            public static string ToWords(double number, IUnit u)
            {
                return ToWords(number, u, Capital.No);
            }

            /// <summary>
            /// Возвращает samples числа.
            /// </summary>
            public static string ToWords(double number, IUnit u, Capital capital)
            {
                return ApplyCaps(ToWords(number, u, new StringBuilder()), capital);
            }

            /// <summary>
            /// Возвращает samples числа строчными буквами.
            /// </summary>
            public static string ToWords(ulong number, IUnit u)
            {
                return ToWords(number, u, Capital.No);
            }

            /// <summary>
            /// Возвращает samples числа.
            /// </summary>
            public static string ToWords(ulong number, IUnit u, Capital capital)
            {
                return ApplyCaps(ToWords(number, u, new StringBuilder()), capital);
            }

            /// <summary>
            /// Возвращает samples числа строчными буквами.
            /// </summary>
            public static string ToWords(uint number, IUnit u)
            {
                return ToWords(number, u, Capital.No);
            }

            /// <summary>
            /// Возвращает samples числа.
            /// </summary>
            public static string ToWords(uint number, IUnit u, Capital capital)
            {
                return ApplyCaps(ToWords(number, u, new StringBuilder()), capital);
            }

            internal static string ApplyCaps(StringBuilder sb, Capital capital)
            {
                capital.Apply(sb);
                return sb.ToString();
            }

            #endregion
        }

        /// <summary>
        /// Стратегия расстановки заглавных букв.
        /// </summary>
        public abstract class Capital
        {
            /// <summary>
            /// Apply стратегию к <paramref name="sb"/>.
            /// </summary>
            public abstract void Apply(StringBuilder sb);

            class _All : Capital
            {
                public override void Apply(StringBuilder sb)
                {
                    for (int i = 0; i < sb.Length; ++i)
                    {
                        sb[i] = char.ToUpperInvariant(sb[i]);
                    }
                }
            }

            class _No : Capital
            {
                public override void Apply(StringBuilder sb)
                {
                }
            }

            class _First : Capital
            {
                public override void Apply(StringBuilder sb)
                {
                    sb[0] = char.ToUpperInvariant(sb[0]);
                }
            }

            public static readonly Capital All = new _All();
            public static readonly Capital No = new _No();
            public static readonly Capital First = new _First();
        }

        /// <summary>
        /// Описывает тип валюты как совокупность двух единиц измерения - основной и дробной.
        /// Содержит несколько предопределённых валют - рубли, доллары, евро.
        /// </summary>
        /// <remarks>
        /// Предполагается, что main единица равна 100 дробным. 
        /// </remarks>
        public class Currency
        {
            /// <summary> </summary>
            public Currency(IUnit main, IUnit fractional)
            {
                this.main = main;
                this.fractional = fractional;
            }

            readonly IUnit main;
            readonly IUnit fractional;

            /// <summary>
            /// Основная единица измерения валюты - рубли, доллары, евро и т.д.
            /// </summary>
            public IUnit MainFractional
            {
                get { return this.main; }
            }

            /// <summary>
            /// Дробная единица измерения валюты - копейки, центы, евроценты и т.д.
            /// </summary>
            public IUnit FractionalUnit
            {
                get { return this.fractional; }
            }

            public static readonly Currency Rubles = new Currency(
                new Unit(GenitiveNumber.Male, "рубль", "рубля", "рублей"),
                new Unit(GenitiveNumber.Female, "копейка", "копейки", "копеек"));

            public static readonly Currency Dollars = new Currency(
                new Unit(GenitiveNumber.Male, "доллар США", "доллара США", "долларов США"),
                new Unit(GenitiveNumber.Male, "цент", "цента", "центов"));

            public static readonly Currency Euro = new Currency(
                new Unit(GenitiveNumber.Male, "евро", "евро", "евро"),
                new Unit(GenitiveNumber.Male, "цент", "цента", "центов"));

            public static readonly Currency Grivna = new Currency(
                new Unit(GenitiveNumber.Male, "гривня", "гривні", "гривень"),
                new Unit(GenitiveNumber.Male, "копійка", "копійки", "копійок"));

            /// <summary>
            /// Возвращает samples суммы строчными буквами.
            /// </summary>
            public string ToWords(decimal summ)
            {
                return Summ.ToWords(summ, this);
            }

            /// <summary>
            /// Возвращает samples суммы строчными буквами.
            /// </summary>
            public string ToWords(double summ)
            {
                return Summ.ToWords(summ, this);
            }

            /// <summary>
            /// Возвращает samples суммы.
            /// </summary>
            public string ToWords(decimal summ, Capital capital)
            {
                return Summ.ToWords(summ, this, capital);
            }

            /// <summary>
            /// Возвращает samples суммы.
            /// </summary>
            public string ToWords(double summ, Capital capital)
            {
                return Summ.ToWords(summ, this, capital);
            }
        }

        /// <summary>
        /// Class, хранящий падежные формы единицы измерения в явном виде.
        /// </summary>
        public class Unit : IUnit
        {
            /// <summary> </summary>
            public Unit(
                GenitiveNumber genitiveNumber,
                string nominativeSingular,
                string genitiveSingular,
                string genitivePlural)
            {
                this.genitiveNumber = genitiveNumber;
                this.nominativeSingular = nominativeSingular;
                this.genitiveSingular = genitiveSingular;
                this.genitivePlural = genitivePlural;
            }

            readonly GenitiveNumber genitiveNumber;
            readonly string nominativeSingular;
            readonly string genitiveSingular;
            readonly string genitivePlural;

            #region IUnit Members

            string IUnit.NominativeSingular
            {
                get { return this.nominativeSingular; }
            }

            string IUnit.GenitiveSingular
            {
                get { return this.genitiveSingular; }
            }

            string IUnit.GenitivePlural
            {
                get { return this.genitivePlural; }
            }

            GenitiveNumber IUnit.GenitiveNumber
            {
                get { return this.genitiveNumber; }
            }

            #endregion
        }

        #region РодЧисло

        /// <summary>
        /// Указывает kind и number.
        /// Может передаваться в качестве параметра "единица измерения" метода 
        /// <see cref="Number.ToWords(decimal,IUnit,StringBuilder)"/>.
        /// Управляет kindом и numberм числительных один и два.
        /// </summary>
        /// <example>
        /// Number.ToWords (2, GenitiveNumber.Male); // "два"
        /// Number.ToWords (2, GenitiveNumber.Female); // "две"
        /// Number.ToWords (21, GenitiveNumber.Neuter); // "двадцать одно"
        /// </example>
        public abstract class GenitiveNumber : IUnit 
        {
            internal abstract string GetForm(IVariesByGender word);

            #region Рода

            class _Male : GenitiveNumber 
            {
                internal override string GetForm(IVariesByGender word)
                {
                    return word.Male;
                }
            }

            class _Female : GenitiveNumber
            {
                internal override string GetForm(IVariesByGender word)
                {
                    return word.Female;
                }
            }

            class _Neuter : GenitiveNumber
            {
                internal override string GetForm(IVariesByGender word)
                {
                    return word.Neuter;
                }
            }

            class _Plural : GenitiveNumber
            {
                internal override string GetForm(IVariesByGender word)
                {
                    return word.Plural;
                }
            }

            public static readonly GenitiveNumber Male = new _Male();
            public static readonly GenitiveNumber Female = new _Female();
            public static readonly GenitiveNumber Neuter = new _Neuter();
            public static readonly GenitiveNumber Plural = new _Plural();

            #endregion

            #region IUnit Members

            GenitiveNumber IUnit.GenitiveNumber
            {
                get { return this; }
            }

            string IUnit.NominativeSingular
            {
                get { return null; }
            }

            string IUnit.GenitiveSingular
            {
                get { return null; }
            }

            string IUnit.GenitivePlural
            {
                get { return null; }
            }

            #endregion
        }

        #region IVariesByGender

        internal interface IVariesByGender 
        {
            string Male { get; }
            string Female { get; }
            string Neuter { get; }
            string Plural { get; }
        }

        #endregion

        #endregion

        #region Unit

        /// <summary>
        /// Представляет единицу измерения (например, метр, рубль)
        /// и содержит всю необходимую информацию для согласования
        /// этой единицы с numberм, а именно - три падежно-numberвых формы
        /// и грамматический kind / number.
        /// </summary>
        public interface IUnit
        {
            /// <summary>
            /// Форма именительного падежа единственного числа.
            /// Согласуется с числительным "один":
            /// одна тысяча, один миллион, один рубль, одни сутки и т.д. 
            /// </summary>
            string NominativeSingular { get; }

            /// <summary>
            /// Форма kindительного падежа единственного числа.
            /// Согласуется с числительными "один, два, три, четыре":
            /// две тысячи, два миллиона, два рубля, двое суток и т.д. 
            /// </summary> 
            string GenitiveSingular { get; }

            /// <summary>
            /// Форма kindительного падежа множественного числа.
            /// Согласуется с числительным "ноль, пять, шесть, семь" и др:
            /// пять тысяч, пять миллионов, пять рублей, пять суток и т.д. 
            /// </summary>
            string GenitivePlural { get; }

            /// <summary>
            /// Род и number единицы измерения.
            /// </summary>
            GenitiveNumber GenitiveNumber { get; }
        }

        #endregion
    }
}
