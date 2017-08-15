//
// Copyright THJ Systems Ltd
//
//------- D E S C R I P T I O N -------
//

//------- P E R F O R M A N C E   P A R A M E T E R S -------
Backtest.Configuration.UseWeekly=true;
Backtest.Configuration.UseQuarterly=true;
Backtest.Configuration.MaxExpirationDTE=45;

if (Backtest.UnderlyingSymbol=="SPX")
{
		Backtest.Configuration.PriceValidation.PositionConfirmationCount=4;
		Backtest.Configuration.PriceValidation.PositionPercChange=8;
}



//------- O P T I M I Z A T I O N   P A R A M E T E R S -------
int PARAM_NearMonth=39;
int PARAM_FarMonth=44;
int PARAM_WingWidth=50;
int PARAM_NumberOfContracts=50;
int PARAM_AdjustUpMoveLimit=75;
int PARAM_AdjustDownMoveLimit=75;
int PARAM_ProfitTarget=15;
int PARAM_MaxLoss=15;
int PARAM_ExitDTE=5;

//------- E N T R Y   R U L E S -------
if(Position.IsOpen==false) {

	Backtest.Tag = Backtest.TradingDateTime.Date;

    //Find the month expiration cycle
    var monthExpiration=GetExpiryByDTE(PARAM_NearMonth, PARAM_FarMonth);
    if (monthExpiration == null) return;   // Haven't found an expiration matching our criteria

    //Create a new Model Position and build an ATM Butterfly using the expiration cycles we found above.
    var modelPosition=NewModelPosition();
	modelPosition.AddButterfly(ATM, PARAM_WingWidth, Buy, Put, PARAM_NumberOfContracts, monthExpiration);
	modelPosition.ClearLeg("LongLegUpper-" + Position.Adjustments);
	var legAsym=CreateModelLeg(BUY,PARAM_NumberOfContracts, GetOptionByStrike(Put, Underlying.Last+40, monthExpiration, true),"LongLegUpper-" + Position.Adjustments);
	modelPosition.AddLeg(legAsym);


    //Flatten Deltas by buying call option(s) with the closest delta.
	double strike=modelPosition.Delta;
	int quantity=1;
	double delta=-70;
	WriteLog("Trade initiation flatten deltas entry point");
	WriteLog("modelPosition.Delta: " + modelPosition.Delta + " max delta per contract: " + delta);
	if (modelPosition.Delta <=  delta)
	{
		WriteLog("Deltas below -70");

		if (modelPosition.Delta >= delta*2)
		{
			WriteLog("Deltas above -140");
			strike = modelPosition.Delta / 2;
			quantity = 2;
		}
		else if (modelPosition.Delta >= delta*3)
		{
			WriteLog("Deltas above -210");
			strike = modelPosition.Delta / 3;
			quantity = 3;
		}
		else if (modelPosition.Delta >= delta*4)
		{
			WriteLog("Deltas above -280");
			strike = modelPosition.Delta / 4;
			quantity = 4;
		}
		else if (modelPosition.Delta >= delta*5)
		{
			WriteLog("Deltas above -350");
			strike = modelPosition.Delta / 5;
			quantity = 5;
		}
		else if (modelPosition.Delta >= delta*6)
		{
			WriteLog("Deltas above -420");
			strike = modelPosition.Delta / 6;
			quantity = 6;
		}
	}

	WriteLog("Deltas to cut: " + modelPosition.Delta + " Strike: " + strike + " quantity: " + quantity);

    var leg=CreateModelLeg(BUY,quantity, GetOptionByDelta(Call, -strike, monthExpiration),"FlattenDeltaLeg-" + Position.Adjustments);
    modelPosition.AddLeg(leg);

	WriteLog("LegName: " + leg.LegName + " LegStike: " + leg.Strike + " LegQty: " + leg.Qty + " Expiry: " + leg.Expiry + " Tag: " + leg.Tag);

    //Commit the Model Position to the Trade Log and add a comment
    modelPosition.CommitTrade("Buy ATM Butterfly");
}

//------- A D J U S T M E N T   R U L E S -------
try
{
	if(Position.IsOpen==true) {

		//WriteLog("Entered Adjustment Rules");

	    //Check if Underlying moved outside of BreakEven limit
	    var midBE = (Position.Expiration().LowerBE + Position.Expiration().UpperBE) / 2;
		var targetLower = midBE - ((midBE - Position.Expiration().LowerBE) * PARAM_AdjustDownMoveLimit / 100);
	    var targetUpper = midBE + ((Position.Expiration().UpperBE - midBE) * PARAM_AdjustUpMoveLimit / 100);
		WriteLog("targetUpper: " + targetUpper + "targetLower: " + targetLower + "Underlying.Last" + Underlying.Last);
	    if (Underlying.Last >= targetUpper) {
	        //Find the farthest Butterfly away from underlying price and remember its adjustment number so we can reference it
	        double diff=0;
	        double diffMax=0;
	        string legName=null;
	        foreach (IPositionLeg leg in Position.GetAllLegs()) {
	            if(leg.LegName.StartsWith("ShortLeg")) {		//we're only looking for the ShortLeg of each Butterfly
	                diff=leg.Strike - Underlying.Last;
	                if (Math.Abs(diff) > diffMax) {
	                    diffMax=diff;
	                    legName=leg.LegName;
	                }
	            }
	        }
	        if (legName!=null) {
	            //Extract the adjustment number
	            string adjustmentID=legName.Substring(legName.LastIndexOf("-") + 1);

	            //Create a new Model Position
	            var modelPosition=NewModelPosition();

	            //Add a new ATM Butterfly using the expiration cycle in Butterfly we are rolling
	            var monthExpiry=GetExpiryByDTE(Position.GetLegByName(legName).DTE);
	            modelPosition.AddButterfly(ATM, PARAM_WingWidth,Position.GetLegByName("LongLegLower-" + adjustmentID).Transaction, Position.GetLegByName("LongLegLower-" + adjustmentID).Type, PARAM_NumberOfContracts, monthExpiry);

				WriteLog("modelPosition.Delta: " + modelPosition.Delta);

			    //Flatten Deltas by buying call option(s) with the closest delta.
				double strike=modelPosition.Delta;
				int quantity=1;
				double delta=-70;
				WriteLog("Upper adjustment point flatten deltas entry");
				WriteLog("modelPosition.Delta: " + modelPosition.Delta + " max delta per contract: " + delta);
				if (modelPosition.Delta <=  delta)
				{
					WriteLog("Deltas below -70");

					if (modelPosition.Delta >= delta*2)
					{
						WriteLog("Deltas above -140");
						strike = modelPosition.Delta / 2;
						quantity = 2;
					}
					else if (modelPosition.Delta >= delta*3)
					{
						WriteLog("Deltas above -210");
						strike = modelPosition.Delta / 3;
						quantity = 3;
					}
					else if (modelPosition.Delta >= delta*4)
					{
						WriteLog("Deltas above -280");
						strike = modelPosition.Delta / 4;
						quantity = 4;
					}
					else if (modelPosition.Delta >= delta*5)
					{
						WriteLog("Deltas above -350");
						strike = modelPosition.Delta / 5;
						quantity = 5;
					}
					else if (modelPosition.Delta >= delta*6)
					{
						WriteLog("Deltas above -420");
						strike = modelPosition.Delta / 6;
						quantity = 6;
					}
				}


				WriteLog("Deltas to cut: " + modelPosition.Delta + " Strike: " + strike + " quantity: " + quantity);

	            var leg2=CreateModelLeg(BUY,quantity, GetOptionByDelta(Call,-strike, monthExpiry),"FlattenDeltaLeg-" + (Position.Adjustments + 1));
	            modelPosition.AddLeg(leg2);

	            //Close all legs of the farthest away Butterfly
	            var leg=Position.GetLegByName("ShortLeg-" + adjustmentID).CreateClosingModelLeg();
				WriteLog("LegName:" + leg.LegName + " LegStike" + leg.Strike + " LegQty" + leg.Qty + " Expiry" + leg.Expiry + " Tag" + leg.Tag);
	            modelPosition.AddLeg(leg);
	            leg=Position.GetLegByName("LongLegLower-" + adjustmentID).CreateClosingModelLeg();
				WriteLog("LegName:" + leg.LegName + " LegStike" + leg.Strike + " LegQty" + leg.Qty + " Expiry" + leg.Expiry + " Tag" + leg.Tag);
	            modelPosition.AddLeg(leg);
	            leg=Position.GetLegByName("LongLegUpper-" + adjustmentID).CreateClosingModelLeg();
				WriteLog("LegName:" + leg.LegName + " LegStike" + leg.Strike + " LegQty" + leg.Qty + " Expiry" + leg.Expiry + " Tag" + leg.Tag);
	            modelPosition.AddLeg(leg);

	            //Commit the Model Position to the Trade Log and add a comment
	            modelPosition.CommitTrade("Roll ATM Butterfly (upside)");
	        }
	    }
	    if (Underlying.Last <= targetLower) {
	        //Find the farthest Butterfly away from underlying price and remember its adjustment number so we can reference it
	        double diff=0;
	        double diffMax=0;
	        string legName=null;
	        foreach (IPositionLeg leg in Position.GetAllLegs()) {
	            if(leg.LegName.StartsWith("ShortLeg")) {		//we're only looking for the ShortLeg of each Butterfly
	                diff=leg.Strike - Underlying.Last;
	                if (Math.Abs(diff) > diffMax) {
	                    diffMax=diff;
	                    legName=leg.LegName;
	                }
	            }
	        }
	        if (legName!=null) {
	            //Extract the adjustment number
	            string adjustmentID=legName.Substring(legName.LastIndexOf("-") + 1);

	            //Create a new Model Position
	            var modelPosition=NewModelPosition();

	            //Add a new ATM Butterfly using the expiration cycle in Butterfly we are rolling
	            var monthExpiry=GetExpiryByDTE(Position.GetLegByName(legName).DTE);
	            modelPosition.AddButterfly(ATM, PARAM_WingWidth,Position.GetLegByName("LongLegLower-" + adjustmentID).Transaction, Position.GetLegByName("LongLegLower-" + adjustmentID).Type, PARAM_NumberOfContracts, monthExpiry);

			    //Flatten Deltas by buying call option(s) with the closest delta.
				double strike=modelPosition.Delta;
				int quantity=1;
				double delta=70;
				WriteLog("Lower adjustment point flatten deltas entry");
				WriteLog("modelPosition.Delta: " + modelPosition.Delta + " max delta per contract: " + delta);
				if (modelPosition.Delta >  delta)
				{
					WriteLog("Deltas over 70");
					delta=140.0;
					if (modelPosition.Delta < delta*2)
					{
						WriteLog("Deltas below 140");
						strike = modelPosition.Delta / 2;
						quantity = 2;
					}
					else if (modelPosition.Delta < delta*3)
					{
						WriteLog("Deltas below 210");
						strike = modelPosition.Delta / 3;
						quantity = 3;
					}
					else if (modelPosition.Delta < delta*4)
					{
						WriteLog("Deltas below 280");
						strike = modelPosition.Delta / 4;
						quantity = 4;
					}
				}

				WriteLog("Deltas to cut: " + modelPosition.Delta + " Strike: " + strike + " quantity: " + quantity);

	            var leg2=CreateModelLeg(BUY,quantity, GetOptionByDelta(Call,-strike, monthExpiry),"FlattenDeltaLeg-" + (Position.Adjustments + 1));
				WriteLog("Flatten Deltas - LegName: " + leg2.LegName + " LegStike: " + leg2.Strike + " LegQty: " + leg2.Qty + " Expiry: " + leg2.Expiry + " Tag: " + leg2.Tag);
	            modelPosition.AddLeg(leg2);

	            //Close all legs of the farthest away Butterfly
	            var leg=Position.GetLegByName("ShortLeg-" + adjustmentID).CreateClosingModelLeg();
	            modelPosition.AddLeg(leg);
	            leg=Position.GetLegByName("LongLegLower-" + adjustmentID).CreateClosingModelLeg();
	            modelPosition.AddLeg(leg);
	            leg=Position.GetLegByName("LongLegUpper-" + adjustmentID).CreateClosingModelLeg();
	            modelPosition.AddLeg(leg);

	            //Commit the Model Position to the Trade Log and add a comment
	            modelPosition.CommitTrade("Roll ATM Butterfly (downside)");
	        }
	    }

	}
}
catch(Exception ex)
{
	WriteLog("Try/Catch hit");
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
    if(Position.Adjustments >= 5) Position.Close("Hit Max Adjustments");

}
