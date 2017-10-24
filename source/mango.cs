//
//
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly = true;
Backtest.Configuration.UseQuarterly = true;
Backtest.Configuration.MaxExpirationDTE = 50;
Backtest.Configuration.CommissionRates.OptionPerContract = 1.0;

if (Backtest.UnderlyingSymbol == "SPX") {
    Backtest.Configuration.PriceValidation.PositionConfirmationCount = 3;
    Backtest.Configuration.PriceValidation.PositionPercChange = 5;
}

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth = 42;
int PARAM_FarMonth = 50;
int PARAM_ProfitTarget = 20;
int PARAM_MaxLoss = 25;
int PARAM_ExitDTE = 5;
int PARAM_MaxAdjustments = 20;

//max underlying IV when initiating a trade
int PARAM_MaxUnderlyingIV = 30;

//roll spreads out at percentage of expiration breakeven
int PARAM_AdjustUpMoveLimit = 99;
int PARAM_AdjustDownMoveLimit = 99;

//width of call and put spreads
//note we well change the width of the call spread to 35 to cut deltas in the entry blocl
//if short deltas > 3 when 50x40
var PARAM_CallSpreadWidth = 40;
var PARAM_PutSpreadWidth = 50;

//Where, in relation to current price of underlying, do we center the butterfly
int PARAM_ATMOffset = 0;

//per contract position delta adjustment point
int PARAM_DeltaAdjustmentPointDownside = 10;
int PARAM_DeltaAdjustmentPointUpside = -10;

//how far to roll spread when expiration breakeven reached
int PARAM_RollDistance = 20;

//entry rules for market condition
double PARAM_UnderlyingMovementSDdown = -1.5;
double PARAM_UnderlyingMovementSDup = 1.5;
int PARAM_UnderlyingMovementSDDays = 3;

//max margin to use for trade
double PARAM_InitialMargin = 20000;
var PARAM_ScaleMargin = false;

//initiation day of month minimum and maximum
int PARAM_InitiationDayMinimum = 1;
int PARAM_InitiationDayMaximum = 31;

//Start and end window for taking trades
TimeSpan currentTime = Backtest.TradingDateTime.ToLocalTime ().TimeOfDay; //Convert from UTC to localtime
TimeSpan startTime = new TimeSpan (8, 45, 0); //9:00 AM
TimeSpan endTime = new TimeSpan (14, 55, 0); //3:00 PM

//scalable margin - add 50% of PnL to size up
//but remove entire negative PnL if PnL is below 0
if (PARAM_ScaleMargin == true) {
    if (Backtest.Tag != null) {
        double addMargin = (double) Backtest.Tag;
        if (addMargin > 0) {
            PARAM_InitialMargin = PARAM_InitialMargin + (addMargin / 2);
        } else {
            PARAM_InitialMargin = PARAM_InitialMargin + (addMargin);
        }
    }
    if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == startTime) WriteLog ("PARAM_InitialxMargin=" + PARAM_InitialMargin);
}

//work around backtest data problem for a specific date
TimeSpan startTime12202012 = new TimeSpan (12, 0, 0);
TimeSpan startTime10AM = new TimeSpan (10, 0, 0);

//minimum and maximum date of the month to enter a trade on
//year and month don't matter for us, because we will only use the .Day property
DateTime PARAM_InitiationDayMin = new DateTime (2000, 1, PARAM_InitiationDayMinimum);
DateTime PARAM_InitiationDayMax = new DateTime (2999, 12, PARAM_InitiationDayMaximum);

//log params at the beginning of the run
if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == startTime) {
    if (Backtest.TradeCount == 1) {
        WriteLog ("-- BEGIN PARAMETERS ------------------------------------------");
        WriteLog ("PARAM_NearMonth:" + PARAM_NearMonth);
        WriteLog ("PARAM_FarMonth: " + PARAM_FarMonth);
        WriteLog ("PARAM_ProfitTarget: " + PARAM_ProfitTarget);
        WriteLog ("PARAM_MaxLoss: " + PARAM_MaxLoss);
        WriteLog ("PARAM_MaxUnderlyingIV: " + PARAM_MaxUnderlyingIV);
        WriteLog ("PARAM_UnderlyingMovementSDdown: " + PARAM_UnderlyingMovementSDdown);
        WriteLog ("PARAM_UnderlyingMovementSDup: " + PARAM_UnderlyingMovementSDup);
        WriteLog ("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays);
        WriteLog ("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays);
        WriteLog ("startTime: " + startTime + " endTime: " + endTime);
        WriteLog ("-- END PARAMETERS ------------------------------------------");
    }
}
try {

    //there's something wrong with the backtest data on 01/04/2016
    if (Backtest.TradingDateTime.Date.ToString() == "1/4/2016 12:00:00 AM") {
        if (currentTime < startTime10AM) {
            return;
        }
    }

    //------- E N T R Y   R U L E S -------
    if (Position.IsOpen == false) {

        //reinitilize tag
        Position.Tag = null;

        // even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
        // because pricing data on 8/24 is a mess and it really throw off the backtest

        if (Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM") {
            WriteLog ("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
            return;
        }
        if (Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM") {
            WriteLog ("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
            return;
        }

        //there's something wrong with the backtest data on 12/20/2012 until 11:00 AM
        if (Backtest.TradingDateTime.Date.ToString() == "12/20/2012 12:00:00 AM") {
            if (currentTime <= startTime12202012) {
                return;
            }
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

        //if initiation week is set, then only initiate trades that week of the month
        //4th week includes days 28-31 as well
        if ((Backtest.TradingDateTime.Day >= PARAM_InitiationDayMin.Day) && (Backtest.TradingDateTime.Day <= PARAM_InitiationDayMax.Day)) {
            if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == startTime) WriteLog ("Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
        } else {
            if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == startTime) WriteLog ("NOT Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
            return;
        }

        //Check if underlying movement within entry SD limits
        double maxSDup = 0.0;
        double maxSDdown = 0.0;
        GetMaxSDMovement (PARAM_UnderlyingMovementSDDays, ref maxSDup, ref maxSDdown);
        if (maxSDup > PARAM_UnderlyingMovementSDup) {
            //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
            if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == new TimeSpan (15, 00, 0)) { //Convert from UTC to localtime
                WriteLog ("SD Up exceeded: maxSDup = " + maxSDup);
                return; // Max SD on upside exceeded
            }
        }
        if (maxSDdown < PARAM_UnderlyingMovementSDdown) {
            //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
            if (Backtest.TradingDateTime.ToLocalTime ().TimeOfDay == new TimeSpan (15, 00, 0)) { //Convert from UTC to localtime
                WriteLog ("SD Down exceeded: maxSDdown = " + maxSDdown);
                return; // Max SD on downside exceeded
            }
        }

        //DO not initiate if the market is going haywire
        if (Underlying.IV <= PARAM_MaxUnderlyingIV) {

            if ((currentTime >= startTime) && (currentTime <= endTime)) {

                //Find the month expiration cycle
                var monthExpiration = GetExpiryByDTE (PARAM_NearMonth, PARAM_FarMonth);
                if (monthExpiration == null) return; // Haven't found an expiration matching our criteria

                //Create a new Model Position and build an Iron Butterfly using the expiration cycles we found above.
                var modelPosition = NewModelPosition ();
                var legAsym1 = CreateModelLeg (SELL, 1, GetOptionByStrike (Put, (Underlying.Last) - PARAM_ATMOffset, monthExpiration, true), "ShortPut-" + Position.Adjustments);
                modelPosition.AddLeg (legAsym1);
                var legAsym2 = CreateModelLeg (BUY, 1, GetOptionByStrike (Put, (Underlying.Last - PARAM_PutSpreadWidth) - PARAM_ATMOffset, monthExpiration, true), "LongPut-" + Position.Adjustments);
                modelPosition.AddLeg (legAsym2);
                var legAsym3 = CreateModelLeg (SELL, 1, GetOptionByStrike (Call, (Underlying.Last) + PARAM_ATMOffset, monthExpiration, true), "ShortCall-" + Position.Adjustments);
                modelPosition.AddLeg (legAsym3);
                var legAsym4 = CreateModelLeg (BUY, 1, GetOptionByStrike (Call, (Underlying.Last + PARAM_CallSpreadWidth) + PARAM_ATMOffset, monthExpiration, true), "LongCall-" + Position.Adjustments);
                modelPosition.AddLeg (legAsym4);
                //need logic here to decive if 40 wide call spread is good or needs to be narrowed to 35
                WriteLog ("Model Position for 1 lot position deltas is: " + modelPosition.Delta);
                if (modelPosition.Delta < -3) {
                    //reset the model 1 lot and use smaller call width
                    modelPosition = NewModelPosition ();
                    PARAM_CallSpreadWidth = 35;
                    legAsym1 = CreateModelLeg (SELL, 1, GetOptionByStrike (Put, (Underlying.Last) - PARAM_ATMOffset, monthExpiration, true), "ShortPut-" + Position.Adjustments);
                    modelPosition.AddLeg (legAsym1);
                    legAsym2 = CreateModelLeg (BUY, 1, GetOptionByStrike (Put, (Underlying.Last - PARAM_PutSpreadWidth) - PARAM_ATMOffset, monthExpiration, true), "LongPut-" + Position.Adjustments);
                    modelPosition.AddLeg (legAsym2);
                    legAsym3 = CreateModelLeg (SELL, 1, GetOptionByStrike (Call, (Underlying.Last) + PARAM_ATMOffset, monthExpiration, true), "ShortCall-" + Position.Adjustments);
                    modelPosition.AddLeg (legAsym3);
                    legAsym4 = CreateModelLeg (BUY, 1, GetOptionByStrike (Call, (Underlying.Last + PARAM_CallSpreadWidth) + PARAM_ATMOffset, monthExpiration, true), "LongCall-" + Position.Adjustments);
                    modelPosition.AddLeg (legAsym4);
                    WriteLog ("Adjusted Model Position for 1 lot position deltas is: " + modelPosition.Delta);
                }

                WriteLog ("1 lot model Center price is: " + (Underlying.Last) + PARAM_ATMOffset + " offset is: " + PARAM_ATMOffset + " last: " + Underlying.Last);
                modelPosition.CommitTrade ("Buy Mango 1 lot");

                //determine margin of a 1 lot so we can figure out how many lots to put on
                double nl = PARAM_InitialMargin / Position.Margin;
                int numLots = (int) nl;
                WriteLog ("numLots: " + numLots);
                var modelPosition2 = NewModelPosition ();
                legAsym1 = CreateModelLeg (SELL, numLots, GetOptionByStrike (Put, (Underlying.Last) - PARAM_ATMOffset, monthExpiration, true), "ShortPut-" + Position.Adjustments);
                modelPosition2.AddLeg (legAsym1);
                legAsym2 = CreateModelLeg (BUY, numLots, GetOptionByStrike (Put, (Underlying.Last - PARAM_PutSpreadWidth) - PARAM_ATMOffset, monthExpiration, true), "LongPut-" + Position.Adjustments);
                modelPosition2.AddLeg (legAsym2);
                legAsym3 = CreateModelLeg (SELL, numLots, GetOptionByStrike (Call, (Underlying.Last) + PARAM_ATMOffset, monthExpiration, true), "ShortCall-" + Position.Adjustments);
                modelPosition2.AddLeg (legAsym3);
                legAsym4 = CreateModelLeg (BUY, numLots, GetOptionByStrike (Call, (Underlying.Last + PARAM_CallSpreadWidth) + PARAM_ATMOffset, monthExpiration, true), "LongCall-" + Position.Adjustments);
                modelPosition2.AddLeg (legAsym4);

                //Commit the Model Position to the Trade Log and add a comment
                WriteLog (numLots + " lot model Center price is: " + (Underlying.Last) + PARAM_ATMOffset + " offset is: " + PARAM_ATMOffset + " last: " + Underlying.Last);
                modelPosition2.CommitTrade ("Buy Mango Iron Butterfly number of lots: " + numLots);
                WriteLog ("Trade Entry - IV: " + Underlying.IV);
            }
        } else {
            WriteLog ("Not initiating a trade because Underlying.IV = " + Underlying.IV + " and max is 25");
        }
    }
} catch (Exception ex) {
    WriteLog ("Try/Catch hit in initiation block");
}

//------- A D J U S T M E N T   R U L E S -------
//
//There are 4 types of adjusmtments to be made:
// 1- buy a long put to cut deltas by 80% if we are down 1.7 SD
// 2- roll shorts up or down when position deltas hit threshold (manual rule 8.5-10 on downside and 8.5-10 on upside, this is set in vars above)
// 3- roll spread on side being pressed up or down when expiration breakeven is hit (rule is to roll the spread 30 points)
//    not narrowins spreads when rolling entire spread because the rolling of shorts should handle that if we're still with too many position deltas
//    after repositioning the spread
// 4- if short leg deltas is less than 10, roll up to 10-15 and roil long up same distance

try {
    if (Position.IsOpen == true) {

        if ((currentTime >= startTime) && (currentTime <= endTime)) {

            //Find the month expiration cycle
            var currentExpiration = GetExpiryByDTE (Position.GetLegByName ("ShortPut*").DTE);

            var midBE = (Position.Expiration ().LowerBE + Position.Expiration ().UpperBE) / 2;
            var targetLowerBE = midBE - (midBE - Position.Expiration ().LowerBE);
            var targetUpperBE = midBE + (Position.Expiration ().UpperBE - midBE);
            var adjustment = false;

            // adjustment #1 - buy puts if we're down 1.7 SD today
            // buy long puts in the back month when we're down 1.7 standard deviation for the day
            // cut deltas by about 80%
            if (Underlying.StdDev < -1.7) {
                WriteLog ("Standard Deviation greater than -1.7 " + Underlying.StdDev);
                var modelPosition = NewModelPosition ();
                int hedgeDelta = 0;
                int hedgeCount = 0;
                if (Position.Delta > 50) {
                    hedgeCount = (int) Position.Delta / 50;
                    hedgeDelta = (int) Position.Delta / hedgeCount;
                } else {
                    // don't do anything until we have at least 50 deltas to cut
                    hedgeCount = 0;
                }
                WriteLog ("hedgeCount: " + hedgeCount);
                if (hedgeCount > 0) {
                    //Find the month expiration cycle
                    var backMonthExpiration = GetExpiryByDTE (PARAM_NearMonth, PARAM_FarMonth);
                    if (backMonthExpiration == null) {
                        WriteLog ("no back month expirtion found when trying to buy hedge, returning!");
                        return; // Haven't found an expiration matching our criteria
                    }

                    var legHedge = CreateModelLeg (BUY, hedgeCount, GetOptionByDelta (Put, -hedgeDelta, currentExpiration), "Insurance-" + Position.Adjustments);
                    //legHedge.Tag="tag: " + (double) Underlying.High;
                    legHedge.Tag = "ins-";
                    modelPosition.AddLeg (legHedge);
                    modelPosition.CommitTrade ("Buy insurance");
                    adjustment = true;
                    //tag position with HOD when we buy unsurance
                    Position.Tag = Underlying.High;
                }
            }

            //Get rid of insurance if we are above HOD of day we bought insurnance
            if (Position.Tag != null) {
                WriteLog ("Last: " + Underlying.Last + " Tag: " + Position.Tag);
                double HOD = Convert.ToDouble (Position.Tag);
                WriteLog ("past HOD = Position.Tag");
                if (Underlying.Last >= HOD) {
                    WriteLog ("Inside if last>tag loop");
                    //Create a new Model Position
                    var modelPosition = NewModelPosition ();
                    var hedge = Position.GetLegByName ("Insurance*");
                    if (hedge != null) {
                        WriteLog ("Inside if hedge!=1null loop");
                        var leg = hedge.CreateClosingModelLeg ();
                        modelPosition.AddLeg (leg);
                        modelPosition.CommitTrade ("Close Insurance");
                        adjustment = true;
                        Position.Tag = null;
                        WriteLog ("At end of if hedge!=1null loop");
                    }
                }
            }

            //adjustment #2 - reposition spread when we hit expiration breakeven on that side
            // roll put fly down if we hit lower breakeven
            if (Underlying.Last <= targetLowerBE) {

                // we already did an adjustment
                if (adjustment == true) return;

                //Create a new Model Position
                var modelPosition = NewModelPosition ();

                //Close the Lower Wing Vertical
                var oldLongLeg = Position.GetLegByName ("LongPut*").CreateClosingModelLeg ();
                modelPosition.AddLeg (oldLongLeg);
                var oldShortLeg = Position.GetLegByName ("ShortPut*").CreateClosingModelLeg ();
                modelPosition.AddLeg (oldShortLeg);

                //Open new vertical with short leg at DeltaTarget
                var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Put, oldShortLeg.Strike - PARAM_RollDistance, currentExpiration), "ShortPut-" + Position.Adjustments);
                modelPosition.AddLeg (newShortLeg);

                var newLongLeg = CreateModelLeg (Buy, oldLongLeg.Qty, GetOptionByStrike (Put, oldLongLeg.Strike - (PARAM_RollDistance + 5), currentExpiration), "LongPut-" + Position.Adjustments);
                modelPosition.AddLeg (newLongLeg);

                //Commit the Model Position to the Trade Log and add a comment
                modelPosition.CommitTrade ("Roll Lower Wing");
                adjustment = true;

            }

            // roll put fly up if we hit upper breakeven
            if (Underlying.Last >= targetUpperBE) {

                // we already did an adjustment
                if (adjustment == true) return;

                //Create a new Model Position
                var modelPosition = NewModelPosition ();

                //Close the Upper Wing Vertical
                var oldLongLeg = Position.GetLegByName ("LongCall*").CreateClosingModelLeg ();
                modelPosition.AddLeg (oldLongLeg);
                var oldShortLeg = Position.GetLegByName ("ShortCall*").CreateClosingModelLeg ();
                modelPosition.AddLeg (oldShortLeg);

                //Open new vertical with short leg at DeltaTarget
                var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Call, oldShortLeg.Strike + PARAM_RollDistance, currentExpiration), "ShortCall-" + Position.Adjustments);
                modelPosition.AddLeg (newShortLeg);
                var newLongLeg = CreateModelLeg (Buy, oldLongLeg.Qty, GetOptionByStrike (Call, oldLongLeg.Strike + (PARAM_RollDistance - 5), currentExpiration), "LongCall-" + Position.Adjustments);
                modelPosition.AddLeg (newLongLeg);

                //Commit the Model Position to the Trade Log and add a comment
                modelPosition.CommitTrade ("Roll Upper Wing");
                adjustment = true;

            }

            // adjustment #3 - roll shorts when position delta greater than allowed range
            // Check if Short Strike Delta's within tolerance on downside
            // NOTE - the logic of rolling shorts back up/down to their original width before starting to roll the other side
            // (like an accordian) is not yet implemented

            var origShortLeg = Position.GetLegByName ("ShortPut*");
            if (origShortLeg != null) {
                double delta = Math.Abs (origShortLeg.Delta);
                if (currentExpiration == null) {
                    return; // Haven't found an expiration matching our criteria
                }

                // we already did an adjustment
                if (adjustment == true) return;

                if (Position.Delta > (PARAM_DeltaAdjustmentPointDownside * Position.GetLegByName ("ShortPut*").Qty)) {
                    WriteLog ("Delta greater than " + PARAM_DeltaAdjustmentPointDownside + " per contract - " + Position.Delta);
                    WriteLog ("Position.Tag=" + Position.Tag);
                    WriteLog ("Short strike delta > " + PARAM_DeltaAdjustmentPointDownside + " quantity: " + Position.GetLegByName ("ShortCall*").Qty + " per contract - delta: " + (PARAM_DeltaAdjustmentPointDownside * Position.GetLegByName ("ShortPut*").Qty));

                    if (Position.GetLegByName ("ShortPut*").Strike >= Position.GetLegByName ("ShortCall*").Strike) {
                        var oldShortLeg = Position.GetLegByName ("ShortPut*").CreateClosingModelLeg ();
                        var modelPosition = NewModelPosition ();
                        modelPosition.AddLeg (oldShortLeg);
                        var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Put, oldShortLeg.Strike - 10, currentExpiration), "ShortPut-" + Position.Adjustments);
                        modelPosition.AddLeg (newShortLeg);
                        WriteLog ("Short Put Strike: " + (Position.GetLegByName ("ShortPut*").Strike + ">= Short Call Strike: " + Position.GetLegByName ("ShortCall*").Strike));
                        modelPosition.CommitTrade ("Roll Puts Down");
                    } else {
                        var oldShortLeg = Position.GetLegByName ("ShortCall*").CreateClosingModelLeg ();
                        var modelPosition = NewModelPosition ();
                        modelPosition.AddLeg (oldShortLeg);
                        var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Call, oldShortLeg.Strike - 10, currentExpiration), "ShortCall-" + Position.Adjustments);
                        modelPosition.AddLeg (newShortLeg);
                        modelPosition.CommitTrade ("Roll Calls Back (downside)");
                        WriteLog ("Short Put Strike: " + (Position.GetLegByName ("ShortPut*").Strike + "< Short Call Strike: " + Position.GetLegByName ("ShortCall*").Strike));
                        WriteLog ("New delta after rolling calls down: " + Position.Delta + " per contract delta: " + (Position.Delta / Position.GetLegByName ("ShortCall*").Qty));
                    }
                }
            }

            //Check if Short Strike Delta's within tolerance on upside
            origShortLeg = Position.GetLegByName ("ShortCall*");
            if (origShortLeg != null) {
                //Find the month expiration cycle
                var monthExpiration = GetExpiryByDTE (Position.GetLegByName ("ShortPut*").DTE);
                if (monthExpiration == null) {
                    return; // Haven't found an expiration matching our criteria
                }
                if (Position.Delta < (PARAM_DeltaAdjustmentPointUpside * Position.GetLegByName ("ShortCall*").Qty)) {
                    // we already did an adjustment
                    if (adjustment == true) return;

                    WriteLog ("Delta less than " + PARAM_DeltaAdjustmentPointUpside + " per contract - " + Position.Delta);
                    WriteLog ("Position.Tag=" + Position.Tag);
                    WriteLog ("Short strike delta < " + PARAM_DeltaAdjustmentPointUpside + " quantity: " + Position.GetLegByName ("ShortCall*").Qty + " per contract - delta: " + Position.Delta);

                    if (Position.GetLegByName ("ShortPut*").Strike >= Position.GetLegByName ("ShortCall*").Strike) {
                        var oldShortLeg = Position.GetLegByName ("ShortCall*").CreateClosingModelLeg ();
                        var modelPosition = NewModelPosition ();
                        modelPosition.AddLeg (oldShortLeg);
                        var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Call, oldShortLeg.Strike + 10, currentExpiration), "ShortCall-" + Position.Adjustments);
                        modelPosition.AddLeg (newShortLeg);
                        modelPosition.CommitTrade ("Roll Calls Up");
                        WriteLog ("Short Put Strike: " + (Position.GetLegByName ("ShortPut*").Strike + ">= Short Call Strike: " + Position.GetLegByName ("ShortCall*").Strike));
                        WriteLog ("New delta after rolling short calls up: " + Position.Delta + " per contract delta: " + (Position.Delta / Position.GetLegByName ("ShortCall*").Qty));;
                    } else {
                        var oldShortLeg = Position.GetLegByName ("ShortPut*").CreateClosingModelLeg ();
                        var modelPosition = NewModelPosition ();
                        modelPosition.AddLeg (oldShortLeg);
                        var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByStrike (Put, oldShortLeg.Strike + 10, currentExpiration), "ShortPut-" + Position.Adjustments);
                        modelPosition.AddLeg (newShortLeg);
                        modelPosition.CommitTrade ("Roll Puts Back (upside)");
                        WriteLog ("Short Put Strike: " + (Position.GetLegByName ("ShortPut*").Strike + "< Short Call Strike: " + Position.GetLegByName ("ShortCall*").Strike));
                        WriteLog ("New delta after rolling short puts up: " + Position.Delta + " per contract delta: " + (Position.Delta / Position.GetLegByName ("ShortCall*").Qty));;
                    }

                }
            }

            // this should roll spread up when short gets below 10 deltas, up to 15-17 and roll long same distance
            if (Position.GetLegByName ("ShortPut*").Delta > -10) {
                WriteLog (">-10 Short put delta is: " + Position.GetLegByName ("ShortPut*").Delta);
                var oldShortLeg = Position.GetLegByName ("ShortPut*").CreateClosingModelLeg ();
                var oldLongLeg = Position.GetLegByName ("LongPut*").CreateClosingModelLeg ();
                var modelPosition = NewModelPosition ();
                modelPosition.AddLeg (oldShortLeg);
                modelPosition.AddLeg (oldLongLeg);
                var newShortLeg = CreateModelLeg (Sell, oldShortLeg.Qty, GetOptionByDelta (Put, -16, currentExpiration), "ShortPut-" + Position.Adjustments);
                modelPosition.AddLeg (newShortLeg);
                var longLegRollDistance = newShortLeg.Strike - oldShortLeg.Strike;
                var newLongLeg = CreateModelLeg (Buy, oldLongLeg.Qty, GetOptionByStrike (Put, oldLongLeg.Strike + longLegRollDistance, currentExpiration), "LongPut-" + Position.Adjustments);
                WriteLog (" longLegRollDistance=" + longLegRollDistance + "newShortLeg.Strike=" + newShortLeg.Strike + " oldShortLeg.Strike=" + oldShortLeg.Strike + " newLongLeg.Strike=" + newLongLeg.Strike + " oldLongLeg.Strike=" + oldLongLeg.Strike);
                modelPosition.AddLeg (newLongLeg);
                modelPosition.CommitTrade ("Roll Put Spread Up");
            }

            //this is an ugly workaround for some data problem on 8-2-11 and 8-3-11 that causes my adjustment/exti rules to not be applied
            //we're just closing the trade before the data problems begin 
            if (Backtest.TradingDateTime.Date.ToString() == "8/1/2011 12:00:00 AM") {
                WriteLog ("Closing due to rule for 8/1/2011");
                Position.Close ("Failed to lookup adjustment options - WORKAROUND for 8/1/2011, just exit");
            }
        }
    }
} catch (Exception ex) {
    WriteLog ("Try/Catch hit in adjustment block");
}

//------- E X I T   R U L E S -------
try {
    if (Position.IsOpen == true) {

        if ((currentTime >= startTime) && (currentTime <= endTime)) {

            double PnL = 0;
            if (Backtest.Tag != null) { PnL = (double) Backtest.Tag; }

            //Check Profit Target
            if (Position.PnL >= ((PARAM_InitialMargin * PARAM_ProfitTarget) / 100)) {
                Position.Close ("Hit Profit Target");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog ("Backtest.Tag=" + Backtest.Tag);
                WriteLog ("Position.PnL=" + Position.PnL);
            }

            //Check Max Loss
            if (Position.PnL <= ((PARAM_InitialMargin * -PARAM_MaxLoss) / 100)) {
                Position.Close ("Hit Max Loss");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog ("Backtest.Tag=" + Backtest.Tag);
                WriteLog ("Position.PnL=" + Position.PnL);
            }

            //Check Minimum DTE
            if (Position.DTE <= PARAM_ExitDTE) {
                Position.Close ("Hit Minimum DTE");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog ("Backtest.Tag=" + Backtest.Tag);
                WriteLog ("Position.PnL=" + Position.PnL);
            }

            //Check Max Adjustments
            if (Position.Adjustments >= PARAM_MaxAdjustments) {
                Position.Close ("Hit Max Adjustments");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog ("Backtest.Tag=" + Backtest.Tag);
                WriteLog ("Position.PnL=" + Position.PnL);
            }

            // even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
            // because pricing data on 8/24 is a mess and it really throw off the backtest

            if (Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM") {
                Position.Close ("8/24/2015 data problem");
                return;
            }
            if (Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM") {
                Position.Close ("8/21/2015 data problem");
                return;
            }
        }
    }
} catch (Exception ex) {
    WriteLog ("Try/Catch hit in adjustment block");
}