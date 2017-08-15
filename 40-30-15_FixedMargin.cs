//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//


//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly=true;
Backtest.Configuration.UseQuarterly=true;
Backtest.Configuration.MaxExpirationDTE=37;
Backtest.Configuration.CommissionRates.OptionPerContract=1.0;

if (Backtest.UnderlyingSymbol=="SPX")
{
            Backtest.Configuration.PriceValidation.PositionConfirmationCount=3;
            Backtest.Configuration.PriceValidation.PositionPercChange=5;
}

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth=30;
int PARAM_FarMonth=36;
int PARAM_NumberOfContracts=10;
int PARAM_ProfitTarget=10;
int PARAM_MaxLoss=12;
int PARAM_ExitDTE=1;
int PARAM_DeltaAdjustTriggerOffset=35;
//initial delta of short strike
int PARAM_DeltaTarget=30;

//max underlying IV when initiating a trade
int PARAM_MaxUnderlyingIV=25;

//entry rules for market condition
double PARAM_UnderlyingMovementSDdown=-1.5;
double PARAM_UnderlyingMovementSDup=1.5;
int PARAM_UnderlyingMovementSDDays=3;

//max margin to use for trade
int PARAM_MaxMargin=50000;

//Do not take action before 9:00 AM
TimeSpan currentTime=Backtest.TradingDateTime.ToLocalTime().TimeOfDay;                //Convert from UTC to localtime
TimeSpan startTime = new TimeSpan(9, 0, 0);                                           //9:00 AM
TimeSpan endTime = new TimeSpan(15, 0, 0);                                            //3:00 PM

//work around backtest data problem for a specific date
TimeSpan startTime12202012 = new TimeSpan(12, 0, 0);



//log params at the beginning of the run
if (Backtest.TradeCount == 1)
{
		WriteLog("-- BEGIN PARAMETERS ------------------------------------------" );
		WriteLog("PARAM_NearMonth:" + PARAM_NearMonth );
		WriteLog("PARAM_FarMonth: " + PARAM_FarMonth );
		WriteLog("PARAM_ProfitTarget: "  + PARAM_ProfitTarget);
		WriteLog("PARAM_MaxLoss: " + PARAM_MaxLoss);
		WriteLog("PARAM_MaxUnderlyingIV: " + PARAM_MaxUnderlyingIV );
		WriteLog("PARAM_UnderlyingMovementSDdown: " + PARAM_UnderlyingMovementSDdown );
		WriteLog("PARAM_UnderlyingMovementSDup: " + PARAM_UnderlyingMovementSDup );
		WriteLog("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays );
		WriteLog("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays );
		WriteLog("PARAM_DeltaTarget: " + PARAM_DeltaTarget);
		WriteLog("PARAM_DeltaAdjustTriggerOffset: " + PARAM_DeltaAdjustTriggerOffset);
		WriteLog("startTime: " + startTime + " endTime: " + endTime );
		WriteLog("-- END PARAMETERS ------------------------------------------" );
}

try
{

	//------- E N T R Y   R U L E S -------
	if(Position.IsOpen==false) {

		// even though it is cheating, don't initiate any trades on 8/21/15 and 8/24/15
		// because pricing data on 8/24 is a mess and it really throw off the backtest

		if (Backtest.TradingDateTime.Date.ToString() == "8/24/2015 12:00:00 AM")
			{
				WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
				return;
			}
		if (Backtest.TradingDateTime.Date.ToString() == "8/21/2015 12:00:00 AM")
			{
				WriteLog("Backtest.TradingDateTime.Date: " + Backtest.TradingDateTime.Date.Date);
				return;
			}

		//there's something wrong with the backtest data on 12/20/2012 until 11:00 AM
		if (Backtest.TradingDateTime.Date.ToString() == "12/20/2012 12:00:00 AM")
			{
				if (currentTime <= startTime12202012) {
					return;
				}
			}


	    //Check if underlying movement within entry SD limits
	    double maxSDup=0.0;
	    double maxSDdown=0.0;
	    GetMaxSDMovement(PARAM_UnderlyingMovementSDDays, ref maxSDup, ref maxSDdown);
	    if (maxSDup > PARAM_UnderlyingMovementSDup)
	      {

	    //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
	    if (Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) {        //Convert from UTC to localtime
	        WriteLog("SD Up exceeded: maxSDup = " + maxSDup );
	            return;         // Max SD on upside exceeded
	      }
	      }
	    if (maxSDdown < PARAM_UnderlyingMovementSDdown)
	            {
	    //Check Time is 8:30 AM otherwise this logs every 5 min but SD value is static for the entire day
	    if (Backtest.TradingDateTime.ToLocalTime().TimeOfDay == new TimeSpan(15, 00, 0)) {        //Convert from UTC to localtime
	            WriteLog("SD Down exceeded: maxSDdown = " + maxSDdown );
	            return;     // Max SD on downside exceeded
	      }
	      }

	      //DO not initiate if the market is going haywire
	      if (Underlying.IV <= PARAM_MaxUnderlyingIV) {

	          if ((currentTime >= startTime) && (currentTime <= endTime)) {

	                //Find the month expiration cycle
	                var monthExpiration=GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
	                if (monthExpiration == null) return;   // Haven't found an expiration matching our criteria

	                //Create a new Model Position and build an ATM Butterfly using the expiration cycles we found above.
	                var modelPosition=NewModelPosition();
	                var legAsym1=CreateModelLeg(BUY,1, GetOptionByDelta(Put, -40, monthExpiration),"LongLegUpper-" + Position.Adjustments);
	                modelPosition.AddLeg(legAsym1);
	                var legAsym2=CreateModelLeg(SELL,2, GetOptionByDelta(Put, -30, monthExpiration),"ShortLeg-" + Position.Adjustments);
	                modelPosition.AddLeg(legAsym2);
	                var legAsym3=CreateModelLeg(BUY,1, GetOptionByDelta(Put, -15, monthExpiration),"LongLegLower-" + Position.Adjustments);
	                modelPosition.AddLeg(legAsym3);
					modelPosition.CommitTrade("Buy 60-40-20 Butterfly 1 lot");

					//determine margin of a 1 lot so we can figure out how many lots to put on
					double nl = PARAM_MaxMargin / Position.Margin;
					int numLots = (int) nl;
	                WriteLog("numLots: " + numLots);
					var modelPosition2=NewModelPosition();
	                legAsym1=CreateModelLeg(BUY,numLots, GetOptionByDelta(Put, -40, monthExpiration),"LongLegUpper-" + Position.Adjustments);
	                modelPosition2.AddLeg(legAsym1);
	                legAsym2=CreateModelLeg(SELL,numLots*2, GetOptionByDelta(Put, -30, monthExpiration),"ShortLeg-" + Position.Adjustments);
	                modelPosition2.AddLeg(legAsym2);
	                legAsym3=CreateModelLeg(BUY,numLots, GetOptionByDelta(Put, -15, monthExpiration),"LongLegLower-" + Position.Adjustments);
	                modelPosition2.AddLeg(legAsym3);

	                //Commit the Model Position to the Trade Log and add a comment
	                modelPosition2.CommitTrade("Buy 60-40-20 Butterfly");
	                WriteLog("Trade Entry - IV: " + Underlying.IV);
	            }
	     }
	      else
	      {
	            WriteLog("Not initiating a trade because Underlying.IV = " + Underlying.IV + " and max is 25");
	      }
	}
}

catch(Exception ex)
{
      WriteLog("Try/Catch hit");
}

//------- A D J U S T M E N T   R U L E S -------
try
{
//------- A D J U S T M E N T   R U L E S -------
if(Position.IsOpen==true) {

    if ((currentTime >= startTime) && (currentTime <= endTime)) {

          //Check if Short Strike Delta's within tollerance
          var origShortLeg=Position.GetLegByName("ShortLeg*");

          //Check if Short Strike Delta's within tollerance
          if (origShortLeg!=null) {
              double delta=Math.Abs(origShortLeg.Delta);
              double deltaTarget=PARAM_DeltaTarget;
                  WriteLog("Downside Check - delta: " + delta + " PARAM_DeltaTarget: " + (PARAM_DeltaTarget + PARAM_DeltaAdjustTriggerOffset) );
              if (delta>PARAM_DeltaTarget + PARAM_DeltaAdjustTriggerOffset) {
              	    WriteLog("Close: Delta change (Downside)");
                	Position.Close("Close: Delta change (Downside)");
              }
          }
      }
}
}
catch(Exception ex)
{
      WriteLog("Try/Catch hit in adjustment block");
}

//------- E X I T   R U L E S -------
if(Position.IsOpen==true) {

      if ((currentTime >= startTime) && (currentTime <= endTime)) {

    //Check Profit Target
    if(Position.PnLPercentage >= PARAM_ProfitTarget) Position.Close("Hit Profit Target");

    //Check Max Loss
    if(Position.PnLPercentage <= -PARAM_MaxLoss) Position.Close("Hit Max Loss");

    //Check Minimum DTE
    if(Position.DTE <= PARAM_ExitDTE) Position.Close("Hit Minimum DTE");

    //Check Max Adjustments
    if(Position.Adjustments >= 10) Position.Close("Hit Max Adjustments");

      }

}
