if (Backtest.UnderlyingSymbol=="SPX")
{
		Backtest.Configuration.PriceValidation.PositionConfirmationCount=4;
		Backtest.Configuration.PriceValidation.PositionPercChange=5;
}

//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly=false;
Backtest.Configuration.UseQuarterly=false;
Backtest.Configuration.MaxExpirationDTE=37;

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth=30;
int PARAM_FarMonth=36;
double PARAM_UnderlyingMovementSDdown=-2;
double PARAM_UnderlyingMovementSDup=2;
int PARAM_UnderlyingMovementSDDays=5;
int PARAM_ShortDelta=10;
int PARAM_WingWidth=20;
int PARAM_NumberOfContracts=10;
int PARAM_ProfitTarget=6;
int PARAM_MaxLoss=12;
int PARAM_ExitDTE=5;

try {

//approximate 50% increase in size each year
int yearcount = Backtest.TradingDateTime.Year - 2010;
if (Backtest.TradingDateTime.Year > 2011) {
	PARAM_NumberOfContracts=PARAM_NumberOfContracts * yearcount / 2;
}

//log params at the beginning of the run
if (Backtest.TradeCount == 1)
{
		WriteLog("-- BEGIN PARAMETERS ------------------------------------------" );	
		WriteLog("PARAM_NearMonth:" + PARAM_NearMonth );
		WriteLog("PARAM_FarMonth: " + PARAM_FarMonth );
		WriteLog("PARAM_ProfitTarget: "  + PARAM_ProfitTarget);
		WriteLog("PARAM_MaxLoss: " + PARAM_MaxLoss);
		WriteLog("PARAM_UnderlyingMovementSDdown: " + PARAM_UnderlyingMovementSDdown );
		WriteLog("PARAM_UnderlyingMovementSDup: " + PARAM_UnderlyingMovementSDup );
		WriteLog("PARAM_UnderlyingMovementSDDays: " + PARAM_UnderlyingMovementSDDays );
		WriteLog("PARAM_ExitDTE: " + PARAM_ExitDTE);
		WriteLog("-- END PARAMETERS ------------------------------------------" );	
}
	
//------- E N T R Y   R U L E S -------
if(Position.IsOpen==false) {

    //Check if underlying movement within entry SD limits
    double maxSDup=0.0;
    double maxSDdown=0.0;
    GetMaxSDMovement(PARAM_UnderlyingMovementSDDays, ref maxSDup, ref maxSDdown);
    if (maxSDup > PARAM_UnderlyingMovementSDup) return;         // Max SD on upside exceeded
    if (maxSDdown < PARAM_UnderlyingMovementSDdown) return;     // Max SD on downside exceeded

    //Find the month expiration cycle
    var monthExpiration=GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
    if (monthExpiration == null) return;   // Haven't found an expiration matching our criteria

    TimeSpan currentTime=Backtest.TradingDateTime.ToLocalTime().TimeOfDay;		        //Convert from UTC to localtime
    TimeSpan startTime = new TimeSpan(9, 0, 0); 							            //10:00 AM
    TimeSpan endTime = new TimeSpan(15, 0, 0); 							            //1:30 PM
    if ((currentTime >= startTime) && (currentTime <= endTime)) {

	    //Create a new Model Position and build a Vertical using the expiration cycles we found above.
	    var modelPosition=NewModelPosition();
	    var shortLeg=CreateModelLeg(SELL, PARAM_NumberOfContracts, GetOptionByDelta(Put, -PARAM_ShortDelta,monthExpiration), "ShortLeg-" + Position.Adjustments);
	    modelPosition.AddLeg(shortLeg);
	    var longLeg=CreateModelLeg(BUY, PARAM_NumberOfContracts, GetOptionByStrike(Put, shortLeg.Strike - PARAM_WingWidth, monthExpiration),"LongLeg-" + Position.Adjustments);
	    modelPosition.AddLeg(longLeg);
	    //Commit the Model Position to the Trade Log and add a comment
	    modelPosition.CommitTrade("Sell Vertical");
	}
}

//------- A D J U S T M E N T   R U L E S -------
//  NONE

//------- E X I T   R U L E S -------
if(Position.IsOpen==true) {

    //Check Profit Target
    if(Position.PnLPercentage >= PARAM_ProfitTarget) Position.Close("Hit Profit Target");

    //Check Max Loss
    if(Position.PnLPercentage <= -PARAM_MaxLoss) Position.Close("Hit Max Loss");

    //Check Minimum DTE
    if(Position.DTE <= PARAM_ExitDTE) Position.Close("Hit Minimum DTE");

    //Check Max Adjustments
    if(Position.Adjustments >= 5) Position.Close("Hit Max Adjustments");

}

} catch (Exception ex) {
 WriteLog("Try/Catch hit");
}