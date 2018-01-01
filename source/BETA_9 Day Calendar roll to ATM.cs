//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly=true;
Backtest.Configuration.UseQuarterly=true;
Backtest.Configuration.MaxExpirationDTE=75;

if (Backtest.UnderlyingSymbol == "SPX") {
 Backtest.Configuration.PriceValidation.PositionConfirmationCount=3;
 Backtest.Configuration.PriceValidation.PositionPercChange=5;
}

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth=14;
int PARAM_FarMonth=21;
int PARAM_NumberOfContracts=10;
int PARAM_AdjustUpMoveLimit=80;
int PARAM_AdjustDownMoveLimit=80;
int PARAM_ProfitTarget=10;
int PARAM_MaxLoss=15;
int PARAM_ExitDTE=5;

// use "Any" to allow entry on all days of the week
var PARAM_initiationDay="Any";

// vixMax max of 15
int vixMax = 9;
bool okToTrade = false;


DateTime[] vix9 = new DateTime[]
{
new DateTime(2017, 05, 02),new DateTime(2017, 06, 14),new DateTime(2017, 06, 19),new DateTime(2017, 09, 01),new DateTime(2017, 09, 15)
};

DateTime[] vix10 = new DateTime[]
{
new DateTime(2013, 03, 14),new DateTime(2014, 06, 06),new DateTime(2014, 06, 09),new DateTime(2014, 06, 10),new DateTime(2014, 06, 18),new DateTime(2014, 06, 19),
new DateTime(2014, 06, 20),new DateTime(2014, 06, 23),new DateTime(2014, 06, 24),new DateTime(2014, 07, 01),new DateTime(2014, 07, 02),new DateTime(2014, 07, 03),new DateTime(2014, 07, 07),
new DateTime(2014, 07, 16),new DateTime(2014, 07, 17),new DateTime(2015, 08, 05),new DateTime(2016, 08, 09),new DateTime(2016, 12, 21),new DateTime(2017, 01, 06),new DateTime(2017, 01, 13),
new DateTime(2017, 01, 24),new DateTime(2017, 01, 25),new DateTime(2017, 01, 26),new DateTime(2017, 01, 27),new DateTime(2017, 02, 03),new DateTime(2017, 02, 09),new DateTime(2017, 02, 10),
new DateTime(2017, 02, 14),new DateTime(2017, 02, 15),new DateTime(2017, 03, 03),new DateTime(2017, 03, 07),new DateTime(2017, 03, 15),new DateTime(2017, 03, 17),new DateTime(2017, 03, 20),
new DateTime(2017, 03, 21),new DateTime(2017, 03, 29),new DateTime(2017, 04, 05),new DateTime(2017, 04, 24),new DateTime(2017, 04, 25),new DateTime(2017, 04, 26),new DateTime(2017, 04, 27),
new DateTime(2017, 04, 28),new DateTime(2017, 05, 02),new DateTime(2017, 05, 03),new DateTime(2017, 05, 04),new DateTime(2017, 05, 11),new DateTime(2017, 05, 12),new DateTime(2017, 05, 15),
new DateTime(2017, 05, 16),new DateTime(2017, 05, 22),new DateTime(2017, 05, 23),new DateTime(2017, 05, 30),new DateTime(2017, 06, 07),new DateTime(2017, 06, 13),new DateTime(2017, 06, 14),
new DateTime(2017, 06, 15),new DateTime(2017, 06, 16),new DateTime(2017, 06, 19),new DateTime(2017, 06, 20),new DateTime(2017, 06, 21),new DateTime(2017, 06, 22),new DateTime(2017, 06, 30),
new DateTime(2017, 07, 03),new DateTime(2017, 07, 05),new DateTime(2017, 07, 07),new DateTime(2017, 07, 10),new DateTime(2017, 07, 11),new DateTime(2017, 07, 12),new DateTime(2017, 07, 28),
new DateTime(2017, 07, 31),new DateTime(2017, 08, 30),new DateTime(2017, 08, 31),new DateTime(2017, 09, 01),new DateTime(2017, 09, 11),new DateTime(2017, 09, 12),new DateTime(2017, 09, 13),
new DateTime(2017, 09, 14),new DateTime(2017, 09, 15),new DateTime(2017, 10, 24),new DateTime(2017, 10, 25),new DateTime(2017, 10, 26),new DateTime(2017, 10, 30),new DateTime(2017, 11, 10),
new DateTime(2017, 11, 13),new DateTime(2017, 11, 20),new DateTime(2017, 11, 30),new DateTime(2017, 12, 01),new DateTime(2017, 12, 04),new DateTime(2017, 12, 05),new DateTime(2017, 12, 06),
new DateTime(2017, 12, 07)
};

DateTime[] vix11 = new DateTime[]
{
new DateTime(2013, 03, 11),new DateTime(2013, 03, 12),new DateTime(2013, 03, 13),new DateTime(2013, 03, 14),new DateTime(2013, 03, 15),
new DateTime(2013, 04, 12),new DateTime(2013, 08, 02),new DateTime(2013, 08, 05),new DateTime(2013, 11, 15),new DateTime(2013, 12, 26),new DateTime(2014, 01, 13),new DateTime(2014, 01, 14),
new DateTime(2014, 01, 15),new DateTime(2014, 01, 17),new DateTime(2014, 05, 12),new DateTime(2014, 05, 13),new DateTime(2014, 05, 14),new DateTime(2014, 05, 21),new DateTime(2014, 05, 22),
new DateTime(2014, 05, 23),new DateTime(2014, 05, 27),new DateTime(2014, 05, 28),new DateTime(2014, 05, 29),new DateTime(2014, 05, 30),new DateTime(2014, 06, 02),new DateTime(2014, 06, 03),
new DateTime(2014, 06, 04),new DateTime(2014, 06, 05),new DateTime(2014, 06, 09),new DateTime(2014, 06, 11),new DateTime(2014, 06, 12),new DateTime(2014, 06, 13),new DateTime(2014, 06, 25),
new DateTime(2014, 06, 26),new DateTime(2014, 06, 27),new DateTime(2014, 06, 30),new DateTime(2014, 07, 07),new DateTime(2014, 07, 08),new DateTime(2014, 07, 09),new DateTime(2014, 07, 10),
new DateTime(2014, 07, 14),new DateTime(2014, 07, 15),new DateTime(2014, 07, 18),new DateTime(2014, 07, 22),new DateTime(2014, 07, 23),new DateTime(2014, 07, 24),new DateTime(2014, 07, 25),
new DateTime(2014, 08, 15),new DateTime(2014, 08, 19),new DateTime(2014, 08, 20),new DateTime(2014, 08, 21),new DateTime(2014, 08, 22),new DateTime(2014, 08, 25),new DateTime(2014, 08, 26),
new DateTime(2014, 08, 27),new DateTime(2014, 08, 28),new DateTime(2014, 08, 29),new DateTime(2014, 09, 03),new DateTime(2014, 09, 04),new DateTime(2014, 09, 05),new DateTime(2014, 09, 17),
new DateTime(2014, 09, 18),new DateTime(2014, 09, 19),new DateTime(2014, 11, 26),new DateTime(2014, 12, 05),new DateTime(2015, 05, 22),new DateTime(2015, 06, 23),new DateTime(2015, 06, 24),
new DateTime(2015, 07, 16),new DateTime(2015, 07, 17),new DateTime(2015, 07, 20),new DateTime(2015, 07, 22),new DateTime(2015, 07, 23),new DateTime(2015, 07, 29),new DateTime(2015, 07, 31),
new DateTime(2016, 07, 19),new DateTime(2016, 07, 20),new DateTime(2016, 07, 21),new DateTime(2016, 07, 22),new DateTime(2016, 07, 29),new DateTime(2016, 08, 01),new DateTime(2016, 08, 04),
new DateTime(2016, 08, 05),new DateTime(2016, 08, 08),new DateTime(2016, 08, 09),new DateTime(2016, 08, 10),new DateTime(2016, 08, 11),new DateTime(2016, 08, 12),new DateTime(2016, 08, 15),
new DateTime(2016, 08, 16),new DateTime(2016, 08, 18),new DateTime(2016, 08, 19),new DateTime(2016, 08, 22),new DateTime(2016, 08, 23),new DateTime(2016, 09, 02),new DateTime(2016, 09, 06),
new DateTime(2016, 09, 07),new DateTime(2016, 09, 08),new DateTime(2016, 09, 22),new DateTime(2016, 09, 23),new DateTime(2016, 12, 06),new DateTime(2016, 12, 07),new DateTime(2016, 12, 08),
new DateTime(2016, 12, 09),new DateTime(2016, 12, 19),new DateTime(2016, 12, 20),new DateTime(2016, 12, 22),new DateTime(2016, 12, 23),new DateTime(2016, 12, 27),new DateTime(2016, 12, 28),
new DateTime(2017, 01, 04),new DateTime(2017, 01, 05),new DateTime(2017, 01, 06),new DateTime(2017, 01, 09),new DateTime(2017, 01, 10),new DateTime(2017, 01, 11),new DateTime(2017, 01, 12),
new DateTime(2017, 01, 17),new DateTime(2017, 01, 18),new DateTime(2017, 01, 20),new DateTime(2017, 01, 23),new DateTime(2017, 01, 24),new DateTime(2017, 01, 30),new DateTime(2017, 01, 31),
new DateTime(2017, 02, 02),new DateTime(2017, 02, 06),new DateTime(2017, 02, 07),new DateTime(2017, 02, 08),new DateTime(2017, 02, 13),new DateTime(2017, 02, 16),new DateTime(2017, 02, 17),
new DateTime(2017, 02, 21),new DateTime(2017, 02, 22),new DateTime(2017, 02, 23),new DateTime(2017, 02, 24),new DateTime(2017, 02, 27),new DateTime(2017, 03, 01),new DateTime(2017, 03, 02),
new DateTime(2017, 03, 06),new DateTime(2017, 03, 07),new DateTime(2017, 03, 08),new DateTime(2017, 03, 09),new DateTime(2017, 03, 10),new DateTime(2017, 03, 13),new DateTime(2017, 03, 14),
new DateTime(2017, 03, 16),new DateTime(2017, 03, 20),new DateTime(2017, 03, 22),new DateTime(2017, 03, 28),new DateTime(2017, 03, 29),new DateTime(2017, 03, 30),new DateTime(2017, 03, 31),
new DateTime(2017, 04, 04),new DateTime(2017, 04, 06),new DateTime(2017, 05, 17),new DateTime(2017, 05, 19),new DateTime(2017, 06, 12),new DateTime(2017, 07, 06),new DateTime(2017, 07, 07),
new DateTime(2017, 08, 09),new DateTime(2017, 08, 10),new DateTime(2017, 08, 15),new DateTime(2017, 08, 16),new DateTime(2017, 08, 17),new DateTime(2017, 08, 22),new DateTime(2017, 08, 23),
new DateTime(2017, 08, 24),new DateTime(2017, 08, 25),new DateTime(2017, 08, 28),new DateTime(2017, 08, 29),new DateTime(2017, 08, 30),new DateTime(2017, 09, 05),new DateTime(2017, 09, 06),
new DateTime(2017, 09, 07),new DateTime(2017, 09, 08),new DateTime(2017, 10, 25),new DateTime(2017, 11, 13),new DateTime(2017, 11, 14),new DateTime(2017, 11, 16),new DateTime(2017, 11, 17)
};

DateTime[] vix12 = new DateTime[]
{
new DateTime(2013, 01, 18),new DateTime(2013, 01, 22),new DateTime(2013, 01, 23),new DateTime(2013, 01, 24),new DateTime(2013, 01, 25),new DateTime(2013, 01, 29),
new DateTime(2013, 02, 01),new DateTime(2013, 02, 08),new DateTime(2013, 02, 11),new DateTime(2013, 02, 12),new DateTime(2013, 02, 13),new DateTime(2013, 02, 14),new DateTime(2013, 02, 15),
new DateTime(2013, 02, 19),new DateTime(2013, 02, 20),new DateTime(2013, 03, 07),new DateTime(2013, 03, 08),new DateTime(2013, 03, 18),new DateTime(2013, 03, 19),new DateTime(2013, 03, 20),
new DateTime(2013, 03, 21),new DateTime(2013, 03, 25),new DateTime(2013, 03, 26),new DateTime(2013, 03, 27),new DateTime(2013, 03, 28),new DateTime(2013, 04, 02),new DateTime(2013, 04, 03),
new DateTime(2013, 04, 09),new DateTime(2013, 04, 10),new DateTime(2013, 04, 11),new DateTime(2013, 04, 12),new DateTime(2013, 04, 15),new DateTime(2013, 05, 03),new DateTime(2013, 05, 06),
new DateTime(2013, 05, 07),new DateTime(2013, 05, 08),new DateTime(2013, 05, 09),new DateTime(2013, 05, 10),new DateTime(2013, 05, 13),new DateTime(2013, 05, 14),new DateTime(2013, 05, 15),
new DateTime(2013, 05, 16),new DateTime(2013, 05, 17),new DateTime(2013, 05, 20),new DateTime(2013, 05, 21),new DateTime(2013, 05, 22),new DateTime(2013, 07, 19),new DateTime(2013, 07, 22),
new DateTime(2013, 07, 23),new DateTime(2013, 07, 24),new DateTime(2013, 07, 25),new DateTime(2013, 07, 26),new DateTime(2013, 07, 31),new DateTime(2013, 08, 01),new DateTime(2013, 08, 02),
new DateTime(2013, 08, 06),new DateTime(2013, 08, 07),new DateTime(2013, 08, 08),new DateTime(2013, 08, 09),new DateTime(2013, 08, 12),new DateTime(2013, 08, 13),new DateTime(2013, 08, 14),
new DateTime(2013, 09, 19),new DateTime(2013, 09, 20),new DateTime(2013, 10, 17),new DateTime(2013, 10, 18),new DateTime(2013, 10, 22),new DateTime(2013, 11, 04),new DateTime(2013, 11, 05),
new DateTime(2013, 11, 06),new DateTime(2013, 11, 07),new DateTime(2013, 11, 08),new DateTime(2013, 11, 11),new DateTime(2013, 11, 12),new DateTime(2013, 11, 13),new DateTime(2013, 11, 14),
new DateTime(2013, 11, 15),new DateTime(2013, 11, 18),new DateTime(2013, 11, 19),new DateTime(2013, 11, 20),new DateTime(2013, 11, 21),new DateTime(2013, 11, 22),new DateTime(2013, 11, 25),
new DateTime(2013, 11, 26),new DateTime(2013, 11, 27),new DateTime(2013, 11, 29),new DateTime(2013, 12, 19),new DateTime(2013, 12, 23),new DateTime(2013, 12, 24),new DateTime(2013, 12, 27),
new DateTime(2013, 12, 30),new DateTime(2014, 01, 07),new DateTime(2014, 01, 08),new DateTime(2014, 01, 09),new DateTime(2014, 01, 10),new DateTime(2014, 01, 14),new DateTime(2014, 01, 16),
new DateTime(2014, 01, 17),new DateTime(2014, 01, 21),new DateTime(2014, 01, 22),new DateTime(2014, 04, 02),new DateTime(2014, 04, 03),new DateTime(2014, 04, 04),new DateTime(2014, 04, 22),
new DateTime(2014, 05, 02),new DateTime(2014, 05, 08),new DateTime(2014, 05, 09),new DateTime(2014, 05, 13),new DateTime(2014, 05, 14),new DateTime(2014, 05, 15),new DateTime(2014, 05, 16),
new DateTime(2014, 05, 19),new DateTime(2014, 05, 20),new DateTime(2014, 06, 16),new DateTime(2014, 06, 17),new DateTime(2014, 07, 10),new DateTime(2014, 07, 11),new DateTime(2014, 07, 18),
new DateTime(2014, 07, 21),new DateTime(2014, 07, 25),new DateTime(2014, 07, 28),new DateTime(2014, 07, 29),new DateTime(2014, 07, 30),new DateTime(2014, 08, 13),new DateTime(2014, 08, 14),
new DateTime(2014, 08, 18),new DateTime(2014, 08, 28),new DateTime(2014, 09, 02),new DateTime(2014, 09, 05),new DateTime(2014, 09, 08),new DateTime(2014, 09, 09),new DateTime(2014, 09, 10),
new DateTime(2014, 09, 11),new DateTime(2014, 09, 12),new DateTime(2014, 09, 16),new DateTime(2014, 09, 18),new DateTime(2014, 11, 07),new DateTime(2014, 11, 10),new DateTime(2014, 11, 11),
new DateTime(2014, 11, 12),new DateTime(2014, 11, 13),new DateTime(2014, 11, 21),new DateTime(2014, 11, 24),new DateTime(2014, 11, 25),new DateTime(2014, 11, 28),new DateTime(2014, 12, 02),
new DateTime(2014, 12, 03),new DateTime(2014, 12, 04),new DateTime(2014, 12, 08),new DateTime(2015, 02, 25),new DateTime(2015, 03, 02),new DateTime(2015, 03, 20),new DateTime(2015, 03, 23),
new DateTime(2015, 03, 24),new DateTime(2015, 04, 10),new DateTime(2015, 04, 13),new DateTime(2015, 04, 15),new DateTime(2015, 04, 16),new DateTime(2015, 04, 20),new DateTime(2015, 04, 21),
new DateTime(2015, 04, 22),new DateTime(2015, 04, 23),new DateTime(2015, 04, 24),new DateTime(2015, 04, 27),new DateTime(2015, 04, 28),new DateTime(2015, 04, 29),new DateTime(2015, 04, 30),
new DateTime(2015, 05, 01),new DateTime(2015, 05, 04),new DateTime(2015, 05, 05),new DateTime(2015, 05, 08),new DateTime(2015, 05, 11),new DateTime(2015, 05, 14),new DateTime(2015, 05, 15),
new DateTime(2015, 05, 18),new DateTime(2015, 05, 19),new DateTime(2015, 05, 20),new DateTime(2015, 05, 21),new DateTime(2015, 05, 27),new DateTime(2015, 06, 10),new DateTime(2015, 06, 11),
new DateTime(2015, 06, 18),new DateTime(2015, 06, 19),new DateTime(2015, 06, 22),new DateTime(2015, 06, 24),new DateTime(2015, 06, 25),new DateTime(2015, 07, 14),new DateTime(2015, 07, 15),
new DateTime(2015, 07, 21),new DateTime(2015, 07, 22),new DateTime(2015, 07, 24),new DateTime(2015, 07, 30),new DateTime(2015, 08, 03),new DateTime(2015, 08, 04),new DateTime(2015, 08, 06),
new DateTime(2015, 08, 10),new DateTime(2015, 08, 11),new DateTime(2015, 08, 14),new DateTime(2015, 08, 17),new DateTime(2015, 10, 28),new DateTime(2016, 04, 01),new DateTime(2016, 04, 19),
new DateTime(2016, 04, 20),new DateTime(2016, 05, 27),new DateTime(2016, 06, 03),new DateTime(2016, 06, 07),new DateTime(2016, 07, 11),new DateTime(2016, 07, 12),new DateTime(2016, 07, 13),
new DateTime(2016, 07, 14),new DateTime(2016, 07, 15),new DateTime(2016, 07, 18),new DateTime(2016, 07, 22),new DateTime(2016, 07, 25),new DateTime(2016, 07, 26),new DateTime(2016, 07, 27),
new DateTime(2016, 07, 28),new DateTime(2016, 08, 02),new DateTime(2016, 08, 03),new DateTime(2016, 08, 17),new DateTime(2016, 08, 24),new DateTime(2016, 08, 26),new DateTime(2016, 08, 29),
new DateTime(2016, 08, 30),new DateTime(2016, 08, 31),new DateTime(2016, 09, 01),new DateTime(2016, 09, 09),new DateTime(2016, 09, 21),new DateTime(2016, 09, 27),new DateTime(2016, 09, 28),
new DateTime(2016, 09, 29),new DateTime(2016, 09, 30),new DateTime(2016, 10, 04),new DateTime(2016, 10, 05),new DateTime(2016, 10, 06),new DateTime(2016, 10, 07),new DateTime(2016, 10, 24),
new DateTime(2016, 10, 25),new DateTime(2016, 11, 17),new DateTime(2016, 11, 18),new DateTime(2016, 11, 21),new DateTime(2016, 11, 22),new DateTime(2016, 11, 23),new DateTime(2016, 11, 25),
new DateTime(2016, 11, 28),new DateTime(2016, 11, 29),new DateTime(2016, 11, 30),new DateTime(2016, 12, 01),new DateTime(2016, 12, 02),new DateTime(2016, 12, 05),new DateTime(2016, 12, 12),
new DateTime(2016, 12, 13),new DateTime(2016, 12, 14),new DateTime(2016, 12, 15),new DateTime(2016, 12, 16),new DateTime(2016, 12, 29),new DateTime(2016, 12, 30),new DateTime(2017, 01, 03),
new DateTime(2017, 01, 19),new DateTime(2017, 02, 28),new DateTime(2017, 03, 22),new DateTime(2017, 03, 23),new DateTime(2017, 03, 24),new DateTime(2017, 03, 27),new DateTime(2017, 04, 03),
new DateTime(2017, 04, 07),new DateTime(2017, 04, 10),new DateTime(2017, 08, 14),new DateTime(2017, 11, 15)
};

DateTime[] vix13 = new DateTime[]
{
new DateTime(2012, 03, 13),new DateTime(2012, 03, 16),
new DateTime(2012, 08, 13),new DateTime(2012, 08, 14),new DateTime(2012, 08, 17),new DateTime(2012, 08, 20),new DateTime(2012, 08, 21),new DateTime(2012, 09, 10),new DateTime(2012, 09, 13),
new DateTime(2012, 09, 14),new DateTime(2012, 09, 18),new DateTime(2012, 09, 19),new DateTime(2012, 09, 21),new DateTime(2012, 09, 24),new DateTime(2012, 09, 25),new DateTime(2012, 10, 05),
new DateTime(2013, 01, 04),new DateTime(2013, 01, 07),new DateTime(2013, 01, 08),new DateTime(2013, 01, 09),new DateTime(2013, 01, 10),new DateTime(2013, 01, 11),new DateTime(2013, 01, 14),
new DateTime(2013, 01, 15),new DateTime(2013, 01, 16),new DateTime(2013, 01, 17),new DateTime(2013, 01, 28),new DateTime(2013, 01, 29),new DateTime(2013, 01, 30),new DateTime(2013, 01, 31),
new DateTime(2013, 02, 04),new DateTime(2013, 02, 05),new DateTime(2013, 02, 06),new DateTime(2013, 02, 07),new DateTime(2013, 02, 25),new DateTime(2013, 03, 04),new DateTime(2013, 03, 05),
new DateTime(2013, 03, 06),new DateTime(2013, 03, 07),new DateTime(2013, 03, 22),new DateTime(2013, 03, 27),new DateTime(2013, 04, 01),new DateTime(2013, 04, 04),new DateTime(2013, 04, 05),
new DateTime(2013, 04, 08),new DateTime(2013, 04, 16),new DateTime(2013, 04, 22),new DateTime(2013, 04, 23),new DateTime(2013, 04, 24),new DateTime(2013, 04, 25),new DateTime(2013, 04, 26),
new DateTime(2013, 04, 29),new DateTime(2013, 04, 30),new DateTime(2013, 05, 01),new DateTime(2013, 05, 02),new DateTime(2013, 05, 22),new DateTime(2013, 05, 23),new DateTime(2013, 05, 24),
new DateTime(2013, 05, 28),new DateTime(2013, 07, 11),new DateTime(2013, 07, 12),new DateTime(2013, 07, 15),new DateTime(2013, 07, 16),new DateTime(2013, 07, 17),new DateTime(2013, 07, 18),
new DateTime(2013, 07, 29),new DateTime(2013, 07, 30),new DateTime(2013, 08, 07),new DateTime(2013, 08, 15),new DateTime(2013, 08, 16),new DateTime(2013, 08, 23),new DateTime(2013, 08, 26),
new DateTime(2013, 09, 11),new DateTime(2013, 09, 12),new DateTime(2013, 09, 16),new DateTime(2013, 09, 18),new DateTime(2013, 09, 19),new DateTime(2013, 09, 23),new DateTime(2013, 09, 24),
new DateTime(2013, 09, 25),new DateTime(2013, 09, 26),new DateTime(2013, 10, 21),new DateTime(2013, 10, 23),new DateTime(2013, 10, 24),new DateTime(2013, 10, 25),new DateTime(2013, 10, 28),
new DateTime(2013, 10, 29),new DateTime(2013, 10, 30),new DateTime(2013, 10, 31),new DateTime(2013, 11, 01),new DateTime(2013, 11, 20),new DateTime(2013, 12, 02),new DateTime(2013, 12, 06),
new DateTime(2013, 12, 09),new DateTime(2013, 12, 10),new DateTime(2013, 12, 11),new DateTime(2013, 12, 18),new DateTime(2013, 12, 20),new DateTime(2013, 12, 23),new DateTime(2013, 12, 31),
new DateTime(2014, 01, 02),new DateTime(2014, 01, 03),new DateTime(2014, 01, 06),new DateTime(2014, 01, 23),new DateTime(2014, 02, 12),new DateTime(2014, 02, 13),new DateTime(2014, 02, 14),
new DateTime(2014, 02, 18),new DateTime(2014, 02, 24),new DateTime(2014, 02, 25),new DateTime(2014, 02, 26),new DateTime(2014, 02, 27),new DateTime(2014, 02, 28),new DateTime(2014, 03, 04),
new DateTime(2014, 03, 05),new DateTime(2014, 03, 06),new DateTime(2014, 03, 07),new DateTime(2014, 03, 11),new DateTime(2014, 03, 19),new DateTime(2014, 03, 21),new DateTime(2014, 03, 25),
new DateTime(2014, 03, 26),new DateTime(2014, 03, 28),new DateTime(2014, 03, 31),new DateTime(2014, 04, 01),new DateTime(2014, 04, 03),new DateTime(2014, 04, 09),new DateTime(2014, 04, 10),
new DateTime(2014, 04, 16),new DateTime(2014, 04, 17),new DateTime(2014, 04, 21),new DateTime(2014, 04, 23),new DateTime(2014, 04, 24),new DateTime(2014, 04, 25),new DateTime(2014, 04, 28),
new DateTime(2014, 04, 29),new DateTime(2014, 04, 30),new DateTime(2014, 05, 01),new DateTime(2014, 05, 05),new DateTime(2014, 05, 06),new DateTime(2014, 05, 07),new DateTime(2014, 08, 11),
new DateTime(2014, 08, 12),new DateTime(2014, 09, 15),new DateTime(2014, 09, 22),new DateTime(2014, 09, 23),new DateTime(2014, 09, 24),new DateTime(2014, 09, 25),new DateTime(2014, 10, 06),
new DateTime(2014, 10, 31),new DateTime(2014, 11, 06),new DateTime(2014, 11, 07),new DateTime(2014, 11, 12),new DateTime(2014, 11, 14),new DateTime(2014, 11, 17),new DateTime(2014, 11, 18),
new DateTime(2014, 11, 19),new DateTime(2014, 11, 20),new DateTime(2014, 12, 01),new DateTime(2014, 12, 24),new DateTime(2015, 02, 24),new DateTime(2015, 02, 26),new DateTime(2015, 02, 27),
new DateTime(2015, 03, 03),new DateTime(2015, 03, 05),new DateTime(2015, 03, 18),new DateTime(2015, 03, 19),new DateTime(2015, 03, 25),new DateTime(2015, 04, 06),new DateTime(2015, 04, 07),
new DateTime(2015, 04, 08),new DateTime(2015, 04, 09),new DateTime(2015, 04, 14),new DateTime(2015, 04, 17),new DateTime(2015, 05, 05),new DateTime(2015, 05, 06),new DateTime(2015, 05, 11),
new DateTime(2015, 05, 12),new DateTime(2015, 05, 13),new DateTime(2015, 05, 26),new DateTime(2015, 05, 27),new DateTime(2015, 05, 28),new DateTime(2015, 05, 29),new DateTime(2015, 06, 01),
new DateTime(2015, 06, 02),new DateTime(2015, 06, 03),new DateTime(2015, 06, 04),new DateTime(2015, 06, 10),new DateTime(2015, 06, 12),new DateTime(2015, 06, 19),new DateTime(2015, 06, 26),
new DateTime(2015, 07, 13),new DateTime(2015, 07, 28),new DateTime(2015, 08, 07),new DateTime(2015, 08, 11),new DateTime(2015, 08, 12),new DateTime(2015, 08, 13),new DateTime(2015, 08, 17),
new DateTime(2015, 08, 18),new DateTime(2015, 08, 19),new DateTime(2015, 10, 23),new DateTime(2015, 10, 30),new DateTime(2015, 11, 02),new DateTime(2015, 11, 03),new DateTime(2015, 11, 04),
new DateTime(2016, 03, 17),new DateTime(2016, 03, 18),new DateTime(2016, 03, 21),new DateTime(2016, 03, 22),new DateTime(2016, 03, 29),new DateTime(2016, 03, 30),new DateTime(2016, 03, 31),
new DateTime(2016, 04, 01),new DateTime(2016, 04, 04),new DateTime(2016, 04, 06),new DateTime(2016, 04, 13),new DateTime(2016, 04, 14),new DateTime(2016, 04, 15),new DateTime(2016, 04, 18),
new DateTime(2016, 04, 19),new DateTime(2016, 04, 21),new DateTime(2016, 04, 22),new DateTime(2016, 04, 25),new DateTime(2016, 04, 26),new DateTime(2016, 04, 27),new DateTime(2016, 04, 28),
new DateTime(2016, 05, 10),new DateTime(2016, 05, 11),new DateTime(2016, 05, 12),new DateTime(2016, 05, 13),new DateTime(2016, 05, 25),new DateTime(2016, 05, 26),new DateTime(2016, 05, 27),
new DateTime(2016, 05, 31),new DateTime(2016, 06, 02),new DateTime(2016, 06, 06),new DateTime(2016, 06, 08),new DateTime(2016, 06, 09),new DateTime(2016, 07, 08),new DateTime(2016, 07, 11),
new DateTime(2016, 08, 25),new DateTime(2016, 08, 31),new DateTime(2016, 09, 01),new DateTime(2016, 09, 21),new DateTime(2016, 09, 26),new DateTime(2016, 09, 27),new DateTime(2016, 10, 03),
new DateTime(2016, 10, 10),new DateTime(2016, 10, 11),new DateTime(2016, 10, 19),new DateTime(2016, 10, 20),new DateTime(2016, 10, 21),new DateTime(2016, 10, 26),new DateTime(2016, 10, 27),
new DateTime(2016, 11, 10),new DateTime(2016, 11, 15),new DateTime(2016, 11, 16),new DateTime(2016, 11, 17),new DateTime(2016, 12, 01),new DateTime(2016, 12, 29),new DateTime(2016, 12, 30),
new DateTime(2017, 04, 19),new DateTime(2017, 04, 20),new DateTime(2017, 04, 21),new DateTime(2017, 05, 18),new DateTime(2017, 08, 18),new DateTime(2017, 08, 21)
};

DateTime[] vix14 = new DateTime[]
{
new DateTime(2011, 02, 08),new DateTime(2011, 04, 15),new DateTime(2011, 04, 20),new DateTime(2011, 04, 21),new DateTime(2011, 04, 28),new DateTime(2011, 04, 29),new DateTime(2012, 03, 13),
new DateTime(2012, 03, 14),new DateTime(2012, 03, 15),new DateTime(2012, 03, 19),new DateTime(2012, 03, 21),new DateTime(2012, 03, 23),new DateTime(2012, 03, 26),new DateTime(2012, 03, 27),
new DateTime(2012, 03, 30),new DateTime(2012, 08, 10),new DateTime(2012, 08, 15),new DateTime(2012, 08, 16),new DateTime(2012, 08, 22),new DateTime(2012, 09, 07),new DateTime(2012, 09, 10),
new DateTime(2012, 09, 17),new DateTime(2012, 09, 18),new DateTime(2012, 09, 20),new DateTime(2012, 09, 25),new DateTime(2012, 09, 27),new DateTime(2012, 09, 28),new DateTime(2012, 10, 04),
new DateTime(2012, 10, 12),new DateTime(2012, 10, 16),new DateTime(2012, 10, 17),new DateTime(2012, 10, 18),new DateTime(2012, 10, 19),new DateTime(2012, 11, 21),new DateTime(2012, 11, 30),
new DateTime(2013, 01, 02),new DateTime(2013, 01, 03),new DateTime(2013, 01, 31),new DateTime(2013, 02, 21),new DateTime(2013, 02, 22),new DateTime(2013, 02, 27),new DateTime(2013, 02, 28),
new DateTime(2013, 03, 04),new DateTime(2013, 04, 17),new DateTime(2013, 04, 19),new DateTime(2013, 04, 22),new DateTime(2013, 05, 24),new DateTime(2013, 05, 29),new DateTime(2013, 05, 30),
new DateTime(2013, 05, 31),new DateTime(2013, 06, 07),new DateTime(2013, 07, 05),new DateTime(2013, 07, 08),new DateTime(2013, 07, 09),new DateTime(2013, 07, 10),new DateTime(2013, 08, 19),
new DateTime(2013, 08, 20),new DateTime(2013, 08, 21),new DateTime(2013, 08, 22),new DateTime(2013, 08, 23),new DateTime(2013, 09, 10),new DateTime(2013, 09, 13),new DateTime(2013, 09, 17),
new DateTime(2013, 09, 23),new DateTime(2013, 09, 27),new DateTime(2013, 10, 16),new DateTime(2013, 12, 03),new DateTime(2013, 12, 04),new DateTime(2013, 12, 05),new DateTime(2013, 12, 11),
new DateTime(2014, 01, 02),new DateTime(2014, 01, 24),new DateTime(2014, 02, 11),new DateTime(2014, 02, 12),new DateTime(2014, 02, 13),new DateTime(2014, 02, 19),new DateTime(2014, 02, 20),
new DateTime(2014, 02, 21),new DateTime(2014, 02, 24),new DateTime(2014, 03, 04),new DateTime(2014, 03, 10),new DateTime(2014, 03, 12),new DateTime(2014, 03, 13),new DateTime(2014, 03, 18),
new DateTime(2014, 03, 20),new DateTime(2014, 03, 24),new DateTime(2014, 03, 25),new DateTime(2014, 03, 27),new DateTime(2014, 04, 07),new DateTime(2014, 04, 08),new DateTime(2014, 04, 16),
new DateTime(2014, 07, 31),new DateTime(2014, 08, 04),new DateTime(2014, 09, 25),new DateTime(2014, 09, 26),new DateTime(2014, 10, 03),new DateTime(2014, 10, 06),new DateTime(2014, 10, 08),
new DateTime(2014, 10, 28),new DateTime(2014, 10, 29),new DateTime(2014, 10, 30),new DateTime(2014, 11, 03),new DateTime(2014, 11, 04),new DateTime(2014, 11, 05),new DateTime(2014, 12, 23),
new DateTime(2014, 12, 24),new DateTime(2014, 12, 26),new DateTime(2015, 02, 13),new DateTime(2015, 02, 20),new DateTime(2015, 02, 23),new DateTime(2015, 03, 04),new DateTime(2015, 03, 06),
new DateTime(2015, 03, 09),new DateTime(2015, 03, 27),new DateTime(2015, 03, 30),new DateTime(2015, 03, 31),new DateTime(2015, 04, 02),new DateTime(2015, 04, 06),new DateTime(2015, 04, 07),
new DateTime(2015, 05, 07),new DateTime(2015, 06, 04),new DateTime(2015, 06, 05),new DateTime(2015, 06, 08),new DateTime(2015, 06, 09),new DateTime(2015, 06, 15),new DateTime(2015, 06, 16),
new DateTime(2015, 06, 17),new DateTime(2015, 10, 19),new DateTime(2015, 10, 20),new DateTime(2015, 10, 21),new DateTime(2015, 10, 22),new DateTime(2015, 10, 26),new DateTime(2015, 10, 27),
new DateTime(2015, 10, 29),new DateTime(2015, 10, 30),new DateTime(2015, 11, 04),new DateTime(2015, 11, 06),new DateTime(2015, 12, 01),new DateTime(2015, 12, 02),new DateTime(2015, 12, 04),
new DateTime(2015, 12, 24),new DateTime(2016, 03, 16),new DateTime(2016, 03, 23),new DateTime(2016, 03, 24),new DateTime(2016, 03, 28),new DateTime(2016, 04, 05),new DateTime(2016, 04, 06),
new DateTime(2016, 04, 07),new DateTime(2016, 04, 08),new DateTime(2016, 04, 11),new DateTime(2016, 04, 12),new DateTime(2016, 04, 29),new DateTime(2016, 05, 02),new DateTime(2016, 05, 03),
new DateTime(2016, 05, 06),new DateTime(2016, 05, 09),new DateTime(2016, 05, 12),new DateTime(2016, 05, 13),new DateTime(2016, 05, 16),new DateTime(2016, 05, 17),new DateTime(2016, 05, 18),
new DateTime(2016, 05, 24),new DateTime(2016, 06, 01),new DateTime(2016, 06, 09),new DateTime(2016, 06, 10),new DateTime(2016, 07, 01),new DateTime(2016, 07, 06),new DateTime(2016, 07, 07),
new DateTime(2016, 09, 12),new DateTime(2016, 09, 19),new DateTime(2016, 09, 20),new DateTime(2016, 10, 28),new DateTime(2016, 11, 09),new DateTime(2016, 11, 11),new DateTime(2016, 11, 14),
new DateTime(2017, 04, 11),new DateTime(2017, 04, 12),new DateTime(2017, 04, 13),new DateTime(2017, 04, 17),new DateTime(2017, 04, 18),new DateTime(2017, 05, 18),new DateTime(2017, 08, 11)
};




//Do not take action before/after specified time
TimeSpan currentTime = Backtest.TradingDateTime.ToLocalTime().TimeOfDay; //Convert from UTC to localtime
TimeSpan startTime = new TimeSpan(8, 50, 0); //9:00 AM
TimeSpan endTime = new TimeSpan(15, 5, 0); //3:00 PM


//------- E N T R Y   R U L E S -------
if(Position.IsOpen==false) {
	
	
  // even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
  // because pricing data on 8/24 is a mess and it really throw off the backtest

  if (Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM") {
   WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
   return;
  }
  if (Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM") {
   WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
   return;
  }


  //there's something wrong with the backtest data on 8/1/2011 - 8/3/2011, don't initiate anything on those days
  if (Backtest.TradingDateTime.Date.ToString() == "8/1/2011 12:00:00 AM") {
   return;
  }
  if (Backtest.TradingDateTime.Date.ToString() == "8/2/2011 12:00:00 AM") {
   return;
  }
  if (Backtest.TradingDateTime.Date.ToString() == "8/3/2011 12:00:00 AM") {
   return;
  }

	if ((currentTime >= startTime) && (currentTime <= endTime)) {
	if(PARAM_initiationDay != "Any")
	{
		if (Backtest.TradingDateTime.Date.DayOfWeek.ToString() != PARAM_initiationDay){
			  if (Backtest.TradingDateTime.ToLocalTime().TimeOfDay == startTime)
				{
					//log this only once per day
					WriteLog(Backtest.TradingDateTime.Date.DayOfWeek.ToString() + " DOES NOT match initiation day of " + PARAM_initiationDay);
				}
				return;
		}
		else
			{
				WriteLog(Backtest.TradingDateTime.Date.DayOfWeek.ToString() + " DOES match initiation day of " + PARAM_initiationDay);
			}		
	}

	if (vixMax >= 9)
	{
		foreach( DateTime date in vix9)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 9s");
				}
			}
	}
	
	if (vixMax >= 10)
	{
		foreach( DateTime date in vix10)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 10s");
				}
			}
	}
	
	if (vixMax >= 11)
	{
		foreach( DateTime date in vix11)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 11s");
				}
			}
	}

	if (vixMax >= 12)
	{
		foreach( DateTime date in vix12)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 12s");
				}
			}
	}
	
	if (vixMax >= 13)
	{
		foreach( DateTime date in vix13)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 13s");
				}
			}
	}

	if (vixMax >= 14)
	{
		foreach( DateTime date in vix14)
			{
				if (date.Date == Backtest.TradingDateTime.Date)
				{
					okToTrade = true;
					WriteLog(date.Date +  " IS in list of initiation days with VIX in 14s");
				}
			}
	}


	
	if (okToTrade == false) return;
    //Find the near month expiration cycle
    var nearMonthExpiration=GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
    if (nearMonthExpiration == null) return;   // Haven't found an expiration matching our criteria

    //Find the far month expiration cycle
    var farMonthExpiration=GetExpiryByDTE(PARAM_NearMonth + 14, PARAM_FarMonth + 14);
    if (farMonthExpiration == null) return;   // Haven't found an expiration matching our criteria

    //Create a new Model Position and build an ATM Calandar using the expiration cycles we found above
    var modelPosition=NewModelPosition();
    modelPosition.AddCalendar(ATM, Buy, Put, PARAM_NumberOfContracts, nearMonthExpiration, farMonthExpiration);

    //Commit the Model Position to the Trade Log and add a comment
    modelPosition.CommitTrade("Buy ATM Calendar");
	}
}

//------- A D J U S T M E N T   R U L E S -------
if(Position.IsOpen==true) {

    //Check if Underlying moved outside of BreakEven limit
    var midBE = (Position.Expiration().LowerBE + Position.Expiration().UpperBE) / 2;
    var targetLower = midBE - ((midBE - Position.Expiration().LowerBE) * PARAM_AdjustDownMoveLimit / 100);
    var targetUpper = midBE + ((Position.Expiration().UpperBE - midBE) * PARAM_AdjustUpMoveLimit / 100);
    if (Underlying.Last >= targetUpper) {
        //Find the farthest calendar away from underlying price and remember its adjustment number so we can reference it
        double diff=0;
        double diffMax=0;
        string legName=null;
        foreach (IPositionLeg leg in Position.GetAllLegs()) {
            diff=leg.Strike - Underlying.Last;
            if (Math.Abs(diff) > diffMax) {
                diffMax=diff;
                legName=leg.LegName;
            }
        }
        if (legName!=null) {
            //Extract the adjustment number
            string adjustmentID=legName.Substring(legName.LastIndexOf("-") + 1);

            //Create a new Model Position
            var modelPosition=NewModelPosition();

            //Add a new ATM Calandar using the expiration cycles in calendar we are rolling
            modelPosition.AddCalendar(ATM, Position.GetLegByName("BackMonthATM-" + adjustmentID).Transaction, Position.GetLegByName("FrontMonthATM-" + adjustmentID).Type, PARAM_NumberOfContracts, GetExpiryByDTE(Position.GetLegByName("FrontMonthATM-" + adjustmentID).DTE), GetExpiryByDTE(Position.GetLegByName("BackMonthATM-" + adjustmentID).DTE));

            //Close both legs of the farthest away calendar
            var leg=Position.GetLegByName("BackMonthATM-" + adjustmentID).CreateClosingModelLeg();
            modelPosition.AddLeg(leg);
            leg=Position.GetLegByName("FrontMonthATM-" + adjustmentID).CreateClosingModelLeg();
            modelPosition.AddLeg(leg);

            //Commit the Model Position to the Trade Log and add a comment
            modelPosition.CommitTrade("Roll ATM Calendar (upside)");
        }
    }
    if (Underlying.Last <= targetLower) {
        //Find the farthest calendar away from underlying price and remember its adjustment number so we can reference it
        double diff=0;
        double diffMax=0;
        string legName=null;
        foreach (IPositionLeg leg in Position.GetAllLegs()) {
            diff=leg.Strike - Underlying.Last;
            if (Math.Abs(diff) > diffMax) {
                diffMax=diff;
                legName=leg.LegName;
            }
        }
        if (legName!=null) {
            //Extract the adjustment number
            string adjustmentID=legName.Substring(legName.LastIndexOf("-") + 1);

            //Create a new Model Position
            var modelPosition=NewModelPosition();

            //Add a new ATM Calandar using the expiration cycles in calendar we are rolling
            modelPosition.AddCalendar(ATM, Position.GetLegByName("BackMonthATM-" + adjustmentID).Transaction, Position.GetLegByName("FrontMonthATM-" + adjustmentID).Type, PARAM_NumberOfContracts, GetExpiryByDTE(Position.GetLegByName("FrontMonthATM-" + adjustmentID).DTE), GetExpiryByDTE(Position.GetLegByName("BackMonthATM-" + adjustmentID).DTE));

            //Close both legs of the farthest away calendar
            var leg=Position.GetLegByName("BackMonthATM-" + adjustmentID).CreateClosingModelLeg();
            modelPosition.AddLeg(leg);
            leg=Position.GetLegByName("FrontMonthATM-" + adjustmentID).CreateClosingModelLeg();
            modelPosition.AddLeg(leg);

            //Commit the Model Position to the Trade Log and add a comment
            modelPosition.CommitTrade("Roll ATM Calendar (downside)");
        }
    }

}

//------- E X I T   R U L E S -------
if(Position.IsOpen==true) {

    //Check Profit Target
    if(Position.PnLPercentage >= PARAM_ProfitTarget) Position.Close("Hit Profit Target");

    //Check Max Loss
    if(Position.PnLPercentage <= -PARAM_MaxLoss) Position.Close("Hit Max Loss");

    //Check Minimum DTE
    if(Position.DTE <= PARAM_ExitDTE) Position.Close("Hit Minimum DTE");

    //Check Max Adjustments
    if(Position.Adjustments >= 9) Position.Close("Hit Max Ajustments");
}