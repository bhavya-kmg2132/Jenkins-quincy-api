namespace Application.Common.Utilities
{
    public class RegexConstant
    {
        public static readonly string email = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,9})$";
        public static readonly string phone = @"^\([0-9]{3}\)\s[0-9]{3}-[0-9]{4}$";
        public static readonly string ein = @"^((?!111111111)(?!222222222)(?!333333333)(?!444444444)(?!555555555)(?!666666666)(?!777777777)(?!888888888)(?!999999999)(?!000000000)(?!123456789)(?!123456789)(?!123123123)([0-9]{9}))*$";
        public static readonly string zipCode = @"^\d{5}(?:[-\s]\d{4})?$";
        //public static readonly string expMod = @"^\d{1}(\.\d{0,1})?$"; //@"^[0-9]{1}.[0-9]{1}$";
        public static readonly string expMod = @"^\d{1}(\.\d{0,2})?$"; //@"^[0-9]{1}.[0-9]{1}$";
        public static readonly string date_MMddYYYYWithSlash = @"^(0[1-9]|1[0-2])\/(0[1-9]|1\d|2\d|3[01])\/(19|20)\d{2}$";
        public static readonly string date_MMddYYYYWithDash = @"^(0[1-9]|1[0-2])-(0[1-9]|1\d|2\d|3[01])-(19|20)\d{2}$";
        public static readonly string time = @"([01]?[0-9]|2[0-3]):[0-5][0-9]$";
        public static readonly string WBA = @"\[[W][B][A]\-[0-9]{4}\]$";
        public static readonly string datetime = @"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d(?:\.\d+)?Z?";

        // ([01]?[0-9]|2[0-3]):[0-5][0-9]

        // ^([0 - 1]?[0 - 9]|2[0-3]):[0-5] [0-9]$
    }
}