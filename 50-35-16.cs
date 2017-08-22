//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//


//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly = true;
Backtest.Configuration.UseQuarterly = true;
Backtest.Configuration.MaxExpirationDTE = 85;
Backtest.Configuration.CommissionRates.OptionPerContract = 1.0;

if (Backtest.UnderlyingSymbol == "SPX") {
 Backtest.Configuration.PriceValidation.PositionConfirmationCount=3;
 Backtest.Configuration.PriceValidation.PositionPercChange=5;
}

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth = 60;
int PARAM_FarMonth = 85;
int PARAM_ProfitTarget = 7;
int PARAM_MaxLoss = 10;
int PARAM_ExitDTE = 30;
int PARAM_DeltaAdjustTriggerOffset = 35;
int PARAM_AdjustDownMoveLimit = 99;
int PARAM_MaxAdjustments=5;

//max underlying IV when initiating a trade
int PARAM_MaxUnderlyingIV = 25;

//entry rules for market condition
double PARAM_UnderlyingMovementSDdown = -1.5;
double PARAM_UnderlyingMovementSDup = 1.5;
int PARAM_UnderlyingMovementSDDays = 3;

//max margin to use for trade
double PARAM_MaxMargin = 50000;
var PARAM_ScaleMargin = true;

//initiation day of month minimum and maximum
int PARAM_InitiationDayMinimum=1;
int PARAM_InitiationDayMaximum=7;

//if we have had to hedge, lower profit target
if (Position.IsOpen == true) {
 foreach(Position.IPositionLeg leg in Position.GetAllLegs()) {
  if (leg.LegName == "Hedge-1") {
   PARAM_ProfitTarget = 3;
   WriteLog("Hedge has been added so reducing profit target to " + PARAM_ProfitTarget);
  }
 }
}

//scalable margin 
 if (PARAM_ScaleMargin == true) {
    if (Backtest.Tag != null) {
	double addMargin = (double)Backtest.Tag;
	PARAM_MaxMargin = PARAM_MaxMargin + (addMargin/2) ;
	}
   WriteLog("PARAM_MaxMargin=" + PARAM_MaxMargin);
  }
 
//Do not take action before 9:00 AM
TimeSpan currentTime = Backtest.TradingDateTime.ToLocalTime().TimeOfDay; //Convert from UTC to localtime
TimeSpan startTime = new TimeSpan(9, 0, 0); //9:00 AM
TimeSpan endTime = new TimeSpan(15, 0, 0); //3:00 PM

//work around backtest data problem for a specific date
TimeSpan startTime12202012 = new TimeSpan(12, 0, 0);

//minimum and maximum date of the month to enter a trade on
//year and month don't matter for us, because we will only use the .Day property
DateTime PARAM_InitiationDayMin = new DateTime(2000, 1, PARAM_InitiationDayMinimum);
DateTime PARAM_InitiationDayMax = new DateTime(2999, 12, PARAM_InitiationDayMaximum);

//log params at the beginning of the run
if (Backtest.TradeCount == 1) {
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
 WriteLog("PARAM_DeltaAdjustTriggerOffset: " + PARAM_DeltaAdjustTriggerOffset);
 WriteLog("startTime: " + startTime + " endTime: " + endTime);
 WriteLog("-- END PARAMETERS ------------------------------------------");
}

try {

 //------- E N T R Y   R U L E S -------
 if (Position.IsOpen == false) {

  //reinitilize tag
  Position.Tag = null;

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
   WriteLog("Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
  } else {
   WriteLog("NOT Ok to take trades this week - Day=" + Backtest.TradingDateTime.Day + " Min=" + PARAM_InitiationDayMin.Day + " Max=" + PARAM_InitiationDayMax.Day);
   return;
  }

  //Check if underlying movement within entry SD limits
  double maxSDup = 0.0;
  double maxSDdown = 0.0;
  GetMaxSDMovement(PARAM_UnderlyingMovementSDDays, ref maxSDup, ref maxSDdown);
  if (maxSDup > PARAM_UnderlyingMovementSDup) {
   //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
   if (Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) { //Convert from UTC to localtime
	WriteLog("SD Up exceeded: maxSDup = " + maxSDup);
	return; // Max SD on upside exceeded
   }
  }
  if (maxSDdown < PARAM_UnderlyingMovementSDdown) {
   //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
   if (Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) { //Convert from UTC to localtime
	WriteLog("SD Down exceeded: maxSDdown = " + maxSDdown);
	return; // Max SD on downside exceeded
   }
  }

  //DO not initiate if the market is going haywire
  if (Underlying.IV <= PARAM_MaxUnderlyingIV) {

   if ((currentTime >= startTime) && (currentTime <= endTime)) {

	//Find the month expiration cycle
	var monthExpiration = GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
	if (monthExpiration == null) return; // Haven't found an expiration matching our criteria

	//Create a new Model Position and build an ATM Butterfly using the expiration cycles we found above.
	var modelPosition = NewModelPosition();
	var legAsym1 = CreateModelLeg(BUY, 1, GetOptionByDelta(Put, -50, monthExpiration), "LongLegUpper-" + Position.Adjustments);
	modelPosition.AddLeg(legAsym1);
	var legAsym2 = CreateModelLeg(SELL, 2, GetOptionByDelta(Put, -35, monthExpiration), "ShortLeg-" + Position.Adjustments);
	modelPosition.AddLeg(legAsym2);
	var legAsym3 = CreateModelLeg(BUY, 1, GetOptionByDelta(Put, -16, monthExpiration), "LongLegLower-" + Position.Adjustments);
	modelPosition.AddLeg(legAsym3);
	modelPosition.CommitTrade("Buy 50-35-20 Butterfly 1 lot");

	//determine margin of a 1 lot so we can figure out how many lots to put on
	double nl = PARAM_MaxMargin / Position.Margin;
	int numLots = (int) nl;
	WriteLog("numLots: " + numLots);
	var modelPosition2 = NewModelPosition();
	legAsym1 = CreateModelLeg(BUY, numLots, GetOptionByDelta(Put, -50, monthExpiration), "LongLegUpper-" + Position.Adjustments);
	modelPosition2.AddLeg(legAsym1);
	legAsym2 = CreateModelLeg(SELL, numLots * 2, GetOptionByDelta(Put, -35, monthExpiration), "ShortLeg-" + Position.Adjustments);
	modelPosition2.AddLeg(legAsym2);
	legAsym3 = CreateModelLeg(BUY, numLots, GetOptionByDelta(Put, -16, monthExpiration), "LongLegLower-" + Position.Adjustments);
	modelPosition2.AddLeg(legAsym3);

	//Commit the Model Position to the Trade Log and add a comment
	modelPosition2.CommitTrade("Buy 50-35-20 Butterfly number of lots: " + numLots);
	WriteLog("Trade Entry - IV: " + Underlying.IV);
   }
  } else {
   WriteLog("Not initiating a trade because Underlying.IV = " + Underlying.IV + " and max is 25");
  }
 }
} catch (Exception ex) {
 WriteLog("Try/Catch hit in initiation block");
}

//------- A D J U S T M E N T   R U L E S -------
try {
  if (Position.IsOpen == true) {

  if ((currentTime >= startTime) && (currentTime <= endTime)) {

   //Check if Short Strike Delta's within tollerance
   var origShortLeg = Position.GetLegByName("ShortLeg*");
   if (origShortLeg != null) {
	double delta = Math.Abs(origShortLeg.Delta);
	//Find the month expiration cycle
	var monthExpiration = GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
	if (monthExpiration == null) {
	 return; // Haven't found an expiration matching our criteria
	}
	if (delta > 50) {
	 WriteLog("Short delta greater than 50 - " + delta);
	 WriteLog("Position.Tag=" + Position.Tag);
	 if (Position.Tag != "hedged") {
	  WriteLog("Short strike delta > 50 - delta: " + delta);
	  var modelPosition = NewModelPosition();
	  int hedgeCount = (int) Position.Delta / 50;
	  var legAsym1 = CreateModelLeg(BUY, hedgeCount, GetOptionByDelta(Put, -50, monthExpiration), "Hedge-" + Position.Adjustments);
	  modelPosition.AddLeg(legAsym1);
	  modelPosition.CommitTrade("Buy back 1 short");
	  Position.Tag = "hedged";
	 } else {
	  WriteLog("Short delta not greater than 50 - " + delta);
	 }
	}
   }

   //this is an ugly workaround for some data problem on 8-2-11 and 8-3-11 that causes my adjustment/exti rules to not be applied
   //we're just closing the trade before the data problems begin 
   if (Backtest.TradingDateTime.Date.ToString() == "8/1/2011 12:00:00 AM") {
	WriteLog("Closing due to rule for 8/1/2011");
	Position.Close("Failed to lookup adjustment options - WORKAROUND for 8/1/2011, just exit");
   }
  }

  //Check if Underlying moved outside of BreakEven limit
  var midBE = (Position.Expiration().LowerBE + Position.Expiration().UpperBE) / 2;
  var targetLower = midBE - ((midBE - Position.Expiration().LowerBE) * PARAM_AdjustDownMoveLimit / 100);
  if (Underlying.Last <= targetLower) {
   Position.Close("Close: Expiration BE Hit (downside)");
  }

  //Close hedge if we reached upper BE again
  var targetUpper = midBE + ((Position.Expiration().UpperBE - midBE));
  if (Underlying.Last >= targetUpper) {
   if (Position.Tag == "hedged") {
	//Create a new Model Position
	var modelPosition = NewModelPosition();
	var hedge = Position.GetLegByName("Hedge*");
	if (hedge != null) {
	 var leg = hedge.CreateClosingModelLeg();
	 modelPosition.AddLeg(leg);
	 modelPosition.CommitTrade("Close hedge");
	 Position.Tag = "";
	}
   }
  }
 }
} catch (Exception ex) {
 WriteLog("Try/Catch hit in adjustment block");
}

//------- E X I T   R U L E S -------
//try {
if (Position.IsOpen == true) {
 
 if ((currentTime >= startTime) && (currentTime <= endTime)) {
 
  double PnL = 0;
  if (Backtest.Tag != null) { PnL = (double) Backtest.Tag;}
	
  //Check Profit Target
  if (Position.PnLPercentage >= PARAM_ProfitTarget) {
	Position.Close("Hit Profit Target");
	Backtest.Tag = PnL + Position.PnL - Position.Commission;
	WriteLog("Backtest.Tag=" + Backtest.Tag);
    WriteLog("Position.PnL=" + Position.PnL);
    }

  //Check Max Loss
  if (Position.PnLPercentage <= -PARAM_MaxLoss) {
	Position.Close("Hit Max Loss");
	Backtest.Tag = PnL + Position.PnL - Position.Commission;
	WriteLog("Backtest.Tag=" + Backtest.Tag);
    WriteLog("Position.PnL=" + Position.PnL);
   	}	

  //Check Minimum DTE
  if (Position.DTE <= PARAM_ExitDTE) {
	Position.Close("Hit Minimum DTE");
	Backtest.Tag = PnL + Position.PnL - Position.Commission;
	WriteLog("Backtest.Tag=" + Backtest.Tag);
    WriteLog("Position.PnL=" + Position.PnL);
	}	

  //Check Max Adjustments
  if (Position.Adjustments >= PARAM_MaxAdjustments) {
	Position.Close("Hit Max Adjustments");
	Backtest.Tag = PnL + Position.PnL - Position.Commission;
	WriteLog("Backtest.Tag=" + Backtest.Tag);
    WriteLog("Position.PnL=" + Position.PnL);
	}	


 }
}
//} catch (Exception ex) {
// WriteLog("Try/Catch hit in adjustment block");
//}
