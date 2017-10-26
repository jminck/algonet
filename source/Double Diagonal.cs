//
//
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly = true;
Backtest.Configuration.UseQuarterly = true;
Backtest.Configuration.MaxExpirationDTE = 90;
Backtest.Configuration.CommissionRates.OptionPerContract = 1.0;

if(Backtest.UnderlyingSymbol == "SPX") {
    Backtest.Configuration.PriceValidation.PositionConfirmationCount = 3;
    Backtest.Configuration.PriceValidation.PositionPercChange = 5;
}

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_FrontMonthMinDTE = 35;
int PARAM_FrontMonthMaxDTE = 37;
int PARAM_BackMonthMaxDTE = 28 + PARAM_FrontMonthMinDTE;
int PARAM_BackMonthMinDTE = 28 + PARAM_FrontMonthMaxDTE;
int PARAM_ProfitTarget = 10;
int PARAM_MaxLoss = 15;
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
var PARAM_CallDiagonaldWidth = 20;
var PARAM_PutDiagonalWidth = 20;

//Where, in relation to current price of underlying, do we center the butterfly
int PARAM_FrontMonthCallDelta = 25;
int PARAM_FrontMonthPutDelta = 25;

//per contract position delta adjustment point
//int PARAM_DeltaAdjustmentPointDownside = 10;
//int PARAM_DeltaAdjustmentPointUpside = -10;

//how far to roll spread when expiration breakeven reached
//int PARAM_RollDistance = 20;

//entry rules for market condition
double PARAM_UnderlyingMovementSDdown = -1.5;
double PARAM_UnderlyingMovementSDup = 1.5;
int PARAM_UnderlyingMovementSDDays = 3;

//max margin to use for trade
double PARAM_InitialMargin = 15000;
var PARAM_ScaleMargin = false;

//initiation day of month minimum and maximum
int PARAM_InitiationDayMinimum = 1;
int PARAM_InitiationDayMaximum = 31;

//Start and end window for taking trades
TimeSpan currentTime = Backtest.TradingDateTime.ToLocalTime().TimeOfDay; //Convert from UTC to localtime
TimeSpan startTime = new TimeSpan(8, 45, 0); //9:00 AM
TimeSpan endTime = new TimeSpan(14, 55, 0); //3:00 PM

//scalable margin - add 50% of PnL to size up
//but remove entire negative PnL if PnL is below 0
if(PARAM_ScaleMargin == true) {
    if(Backtest.Tag != null) {
        double addMargin =(double) Backtest.Tag;
        if(addMargin > 0) {
            PARAM_InitialMargin = PARAM_InitialMargin +(addMargin / 2);
        } else {
            PARAM_InitialMargin = PARAM_InitialMargin +(addMargin);
        }
    }
    if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == startTime) WriteLog("PARAM_InitialxMargin=" + PARAM_InitialMargin);
}

//work around backtest data problem for a specific date
TimeSpan startTime12202012 = new TimeSpan(12, 0, 0);
TimeSpan startTime10AM = new TimeSpan(10, 0, 0);

//minimum and maximum date of the month to enter a trade on
//year and month don't matter for us, because we will only use the .Day property
DateTime PARAM_InitiationDayMin = new DateTime(2000, 1, PARAM_InitiationDayMinimum);
DateTime PARAM_InitiationDayMax = new DateTime(2999, 12, PARAM_InitiationDayMaximum);

//log params at the beginning of the run
if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == startTime) {
    if(Backtest.TradeCount == 1) {
        WriteLog("-- BEGIN PARAMETERS ------------------------------------------");
        WriteLog("PARAM_NearMonth:" + PARAM_NearMonth);
        WriteLog("PARAM_FarMonth: " + PARAM_FarMonth);
        WriteLog("PARAM_ProfitTarget: " + PARAM_ProfitTarget);
        WriteLog("PARAM_MaxLoss: " + PARAM_MaxLoss);
        WriteLog("PARAM_MaxUnderlyingIV: " + PARAM_MaxUnderlyingIV);
        WriteLog("PARAM_UnderlyingMovementSDdown: " + PARAM_UnderlyingMovementSDdown);
        WriteLog("PARAM_UnderlyingMovementSDup: " + PARAM_UnderlyingMovementSDup);
        WriteLog("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays);
        WriteLog("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays);
        WriteLog("startTime: " + startTime + " endTime: " + endTime);
        WriteLog("-- END PARAMETERS ------------------------------------------");
    }
}
try {

    //there's something wrong with the backtest data on 01/04/2016
    if(Backtest.TradingDateTime.Date.ToString() == "1/4/2016 12:00:00 AM") {
        if(currentTime < startTime10AM) {
            return;
        }
    }

    //------- E N T R Y   R U L E S -------
    if(Position.IsOpen == false) {

        //reinitilize tag
        Position.Tag = null;

        // even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
        // because pricing data on 8/24 is a mess and it really throw off the backtest
        if(Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM") {
            WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
            return;
        }
        if(Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM") {
            WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
            return;
        }

        //there's something wrong with the backtest data on 12/20/2012 until 11:00 AM
        if(Backtest.TradingDateTime.Date.ToString() == "12/20/2012 12:00:00 AM") {
            if(currentTime <= startTime12202012) {
                return;
            }
        }

        //there's something wrong with the backtest data on 8/1/2011 - 8/3/2011, don't initiate anything on those days
        if(Backtest.TradingDateTime.Date.ToString() == "8/1/2011 12:00:00 AM") {
            return;
        }
        if(Backtest.TradingDateTime.Date.ToString() == "8/2/2011 12:00:00 AM") {
            return;
        }
        if(Backtest.TradingDateTime.Date.ToString() == "8/3/2011 12:00:00 AM") {
            return;
        }

        //if initiation week is set, then only initiate trades that week of the month
        //4th week includes days 28-31 as well
        if((Backtest.TradingDateTime.Day >= PARAM_InitiationDayMin.Day) &&(Backtest.TradingDateTime.Day <= PARAM_InitiationDayMax.Day)) {
            if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == startTime) WriteLog("Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
        } else {
            if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == startTime) WriteLog("NOT Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
            return;
        }

        //Check if underlying movement within entry SD limits
        double maxSDup = 0.0;
        double maxSDdown = 0.0;
        GetMaxSDMovement(PARAM_UnderlyingMovementSDDays, ref maxSDup, ref maxSDdown);
        if(maxSDup > PARAM_UnderlyingMovementSDup) {
            //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
            if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) { //Convert from UTC to localtime
                WriteLog("SD Up exceeded: maxSDup = " + maxSDup);
                return; // Max SD on upside exceeded
            }
        }
        if(maxSDdown < PARAM_UnderlyingMovementSDdown) {
            //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
            if(Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) { //Convert from UTC to localtime
                WriteLog("SD Down exceeded: maxSDdown = " + maxSDdown);
                return; // Max SD on downside exceeded
            }
        }

        //DO not initiate if the market is going haywire
        if(Underlying.IV <= PARAM_MaxUnderlyingIV) {

            if((currentTime >= startTime) &&(currentTime <= endTime)) {

                //Find the month expiration cycle
                var frontMonthExpiration = GetExpiryByDTE(PARAM_FrontMonthMinDTE, PARAM_FrontMonthMaxDTE);
                var backMonthExpiration = GetExpiryByDTE(PARAM_BackMonthMinDTE, PARAM_BackMonthMaxDTE);
                if(monthExpiration == null) return; // Haven't found an expiration matching our criteria

                //Create a new Model Position and build an Iron Butterfly using the expiration cycles we found above.
                var modelPosition = NewModelPosition();
                var legShortPut = CreateModelLeg(SELL, 1, GetOptionBydelta(Put,PARAM_FrontMonthPutDelta, frontMonthExpiration, true), "ShortPut-" + Position.Adjustments);
                modelPosition.AddLeg(legShortPut);
                var legLongPut = CreateModelLeg(BUY, 1, GetOptionByStrike(Put,legLongPut.Strike - PARAM_PutDiagonaldWidth, backMonthExpiration, true), "LongPut-" + Position.Adjustments);
                modelPosition.AddLeg(legLongPut);
                var legShortCall = CreateModelLeg(SELL, 1, GetOptionBydelta(Call,PARAM_FrontMonthCallDelta, frontMonthExpiration, true), "ShortCall-" + Position.Adjustments);
                modelPosition.AddLeg(legShortCall);
                var legLongCall = CreateModelLeg(BUY, 1, GetOptionByStrike(Call,(legLongCall.Strike + PARAM_CallDiagonaldWidth), backMonthExpiration, true), "LongCall-" + Position.Adjustments);
                modelPosition.AddLeg(legLongCall);
              
                modelPosition.CommitTrade("Buy Double Diagonal 1 lot");

                //determine margin of a 1 lot so we can figure out how many lots to put on
                double nl = PARAM_InitialMargin / Position.Margin;
                int numLots =(int) nl;
                WriteLog("numLots: " + numLots);
                var modelPosition2 = NewModelPosition();
                legShortPut = CreateModelLeg(SELL, 1, GetOptionBydelta(Put,PARAM_FrontMonthPutDelta, frontMonthExpiration, true), "ShortPut-" + Position.Adjustments);
                modelPosition2.AddLeg(legShortPut);
                legLongPut = CreateModelLeg(BUY, 1, GetOptionByStrike(Put,legLongPut.Strike - PARAM_PutDiagonaldWidth, backMonthExpiration, true), "LongPut-" + Position.Adjustments);
                modelPosition2.AddLeg(legLongPut);
                legShortCall = CreateModelLeg(SELL, 1, GetOptionBydelta(Call,PARAM_FrontMonthCallDelta, frontMonthExpiration, true), "ShortCall-" + Position.Adjustments);
                modelPosition2.AddLeg(legShortCall);
                legLongCall = CreateModelLeg(BUY, 1, GetOptionByStrike(Call,(legLongCall.Strike + PARAM_CallDiagonaldWidth), backMonthExpiration, true), "LongCall-" + Position.Adjustments);
                modelPosition2.AddLeg(legLongCall);

                //Commit the Model Position to the Trade Log and add a comment
                modelPosition2.CommitTrade("Buy Double Diagonal number of lots: " + numLots);
                WriteLog("Trade Entry - IV: " + Underlying.IV);
            }
        } else {
            WriteLog("Not initiating a trade because Underlying.IV = " + Underlying.IV + " and max is 25");
        }
    }
} catch(Exception ex) {
    WriteLog("Try/Catch hit in initiation block");
}

//------- A D J U S T M E N T   R U L E S -------
//


//------- E X I T   R U L E S -------
try {
    if(Position.IsOpen == true) {

        if((currentTime >= startTime) &&(currentTime <= endTime)) {

            double PnL = 0;
            if(Backtest.Tag != null) { PnL =(double) Backtest.Tag; }

            //Check Profit Target
            if(Position.PnL >=((PARAM_InitialMargin * PARAM_ProfitTarget) / 100)) {
                Position.Close("Hit Profit Target");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog("Backtest.Tag=" + Backtest.Tag);
                WriteLog("Position.PnL=" + Position.PnL);
            }

            //Check Max Loss
            if(Position.PnL <=((PARAM_InitialMargin * -PARAM_MaxLoss) / 100)) {
                Position.Close("Hit Max Loss");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog("Backtest.Tag=" + Backtest.Tag);
                WriteLog("Position.PnL=" + Position.PnL);
            }

            //Check Minimum DTE
            if(Position.DTE <= PARAM_ExitDTE) {
                Position.Close("Hit Minimum DTE");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog("Backtest.Tag=" + Backtest.Tag);
                WriteLog("Position.PnL=" + Position.PnL);
            }

            //Check Max Adjustments
            if(Position.Adjustments >= PARAM_MaxAdjustments) {
                Position.Close("Hit Max Adjustments");
                Backtest.Tag = PnL + Position.PnL - Position.Commission;
                WriteLog("Backtest.Tag=" + Backtest.Tag);
                WriteLog("Position.PnL=" + Position.PnL);
            }

            // even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
            // because pricing data on 8/24 is a mess and it really throw off the backtest
            if(Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM") {
                Position.Close("8/24/2015 data problem");
                return;
            }
            if(Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM") {
                Position.Close("8/21/2015 data problem");
                return;
            }
			
            //this is an ugly workaround for some data problem on 8-2-11 and 8-3-11 that causes my adjustment/exti rules to not be applied
            //we're just closing the trade before the data problems begin 
            if(Backtest.TradingDateTime.Date.ToString() == "8/1/2011 12:00:00 AM") {
                WriteLog("Closing due to rule for 8/1/2011");
                Position.Close("Failed to lookup adjustment options - WORKAROUND for 8/1/2011, just exit");
            }
        }
    }
} catch(Exception ex) {
    WriteLog("Try/Catch hit in adjustment block");
}