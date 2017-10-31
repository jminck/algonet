if (Backtest.UnderlyingSymbol=="SPX")
{
		Backtest.Configuration.PriceValidation.PositionConfirmationCount=3;
		Backtest.Configuration.PriceValidation.PositionPercChange=5;
}

//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly=true;
Backtest.Configuration.UseQuarterly=false;
Backtest.Configuration.MaxExpirationDTE=5; //maake larger than farmonth

//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth = 3;  // in ONE, the Fri expiry is 4 days away from Mon
int PARAM_FarMonth = 6;
int PARAM_WingWidth = 30;
int PARAM_NumberOfContracts = 10;
int PARAM_ProfitTarget = 20;
int PARAM_MaxLoss = 30;
int PARAM_ExitDTE = 1;  //max days to expiry - get out how many days before expiry?


try {

//------- E N T R Y   R U L E S -------
if(Position.IsOpen==false) {

	 WriteLog("-- BEGIN PARAMETERS ------------------------------------------");
	 WriteLog("PARAM_NearMonth:" + PARAM_NearMonth);
	 WriteLog("PARAM_FarMonth: " + PARAM_FarMonth);
	 WriteLog("-- END PARAMETERS ------------------------------------------");

	
    //Find the weekly expiration cycle
    var monthExpiration=GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
    if (monthExpiration == null) return;   // Haven't found an expiration matching our criteria
	
	TimeSpan currentTime=Backtest.TradingDateTime.ToLocalTime().TimeOfDay;		    //Convert from UTC to localtime
    TimeSpan TradeTime = new TimeSpan(9, 30, 0); 							        //10:00 AM
	
	if (Backtest.TradingDateTime.DayOfWeek != DayOfWeek.Monday) return;
	
    if (currentTime == TradeTime) {

	    //Create a new Model Position and build an ATM Call Butterfly using the expiration cycles we found above.
	    var modelPosition=NewModelPosition();
	    modelPosition.AddButterfly(ATM, PARAM_WingWidth, Buy, Call, PARAM_NumberOfContracts, monthExpiration);

	    //Commit the Model Position to the Trade Log and add a comment
	    modelPosition.CommitTrade("Buy ATM Call Butterfly");
	}
}

//------- A D J U S T M E N T   R U L E S -------
// none


//------- E X I T   R U L E S -------
if(Position.IsOpen==true) {
	
var shortLeg=Position.GetLegByName("ShortLeg-*");
var whichBE="";
	if(shortLeg.Strike > Underlying.Last)
	{
		//get lower BE price 
		if (Position.Expiration().LowerBE > 0)
			{
			Position.Tag = Position.Expiration().LowerBE;
			whichBE = "Lower";
			}
	} else {
		//get upper BE price 
		if (Position.Expiration().UpperBE > 0)
			{
			Position.Tag = Position.Expiration().UpperBE;
			whichBE = "Upper";
			}
	}
	
//debug: my position breakevens went to zero. why???
WriteLog("=================== Exit preamble ===================");
WriteLog("Underlying.Last: " + Underlying.Last);
WriteLog("Position.Expiration().UpperBE: " + Position.Expiration().UpperBE);
WriteLog("Position.Expiration().LowerBE: " + Position.Expiration().LowerBE);
WriteLog("Position.Tag: " + Position.Tag);
WriteLog("whichBE: " + whichBE);
WriteLog("PnLPercentage: " + Position.PnLPercentage);


//if (Position.Expiration().UpperBE > 0) {

	//Check Profit Target
    if(Position.PnLPercentage >= PARAM_ProfitTarget) Position.Close("Hit Profit Target");

    //Check Max Loss
    if(Position.PnLPercentage <= -PARAM_MaxLoss) Position.Close("Hit Max Loss");

	//Check Minimum DTE
    //if(Position.DTE <= PARAM_ExitDTE) Position.Close("Hit Minimum DTE");

	//Check if Underlying moved outside of Upper BreakEven limit
	if (whichBE == "Upper") {
	    if (Underlying.Last >= (double) Position.Tag) {
		    WriteLog("=================== Upper logic ===================");
		    WriteLog("Underlying.Last:" + Underlying.Last);
		    WriteLog("Position.Expiration().UpperBE: " + Position.Expiration().UpperBE);
			WriteLog("PnLPercentage: " + Position.PnLPercentage);
			WriteLog("Position.Tag: " + Position.Tag);
			
			//close if losing more than 10%
		    if(Position.PnLPercentage <= -10) {
				WriteLog("Outside upper breakeven and P&L > -10%");
				WriteLog("PnLPercentage: " + Position.PnLPercentage);
				Position.Close("Outside upper breakeven and P&L > -10%");
			} else {
				WriteLog("Expiration BE Hit - upper side");
				WriteLog("PnLPercentage: " + Position.PnLPercentage);
			}
		}
	}

	//Check if Underlying moved outside of Lower BreakEven limit
	if (whichBE == "Lower") {
		    if (Underlying.Last <= (double) Position.Tag) {

			WriteLog("=================== Lower logic ===================");
			WriteLog("Underlying.Last:" + Underlying.Last);
			WriteLog("Position.Expiration().LowerBE: " + Position.Expiration().LowerBE);
			WriteLog("PnLPercentage: " + Position.PnLPercentage);
			WriteLog("Position.Tag: " + Position.Tag);
				
			//close if losing more than 10%
		    if(Position.PnLPercentage <= -10) {
				WriteLog("Outside lower breakeven and P&L > -10%");
				WriteLog("PnLPercentage: " + Position.PnLPercentage);
				Position.Close("Outside lower breakeven and P&L > -10%");
			} else {
				WriteLog("Expiration BE Hit - lower side");
				WriteLog("PnLPercentage: " + Position.PnLPercentage);
			}
		}	
	}
//}
}

} catch (Exception ex) {
 WriteLog("Try/Catch hit");
}