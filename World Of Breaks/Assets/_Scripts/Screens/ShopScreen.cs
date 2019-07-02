//using system.collections;
//using system.collections.generic;
//using unityengine;
//using unityengine.ui;
//using unityengine.purchasing;

//public class shopscreen : screenbase
//{

//    public gameobject itemsholder;
//    public button openboxbtn;

//    public scrollsnap scrollsnap;

//    public int curactiveitem
//    {
//        get
//        {
//            return scrollsnap.getcuritemindex;
//        }
//    }

//    public int itemcount
//    {
//        get
//        {
//            return scrollsnap.items.length;
//        }
//    }

//    public text itemnametext;
//    public text itemabilitytext;
//    public text itembiomeinfotext;

//    public button selectandplaybtn;

//    public button buybtn;

//    public override void init()
//    {
//        scrollsnap.init();

//        scrollsnap.onchangeitemevent += onchangeitem;

//        selectandplaybtn.onclick.removealllisteners();
//        selectandplaybtn.onclick.addlistener(() =>
//        {
//            selectandplay(curactiveitem);
//            scenecontroller.restartlevel();
//        });

//        buybtn.onclick.removealllisteners();
//        buybtn.onclick.addlistener(() =>
//        {
//            buypaiditem(curactiveitem);
//        });

//        for (int i = 0; i < itemcount; i++)
//        {
//            scrollsnap.setitemstate(i, user.getinfo.userdata[i].bought);
//        }


//        purchasemanager.onpurchasenonconsumable += buypaiditemsuccess;

//    }

//    public override void onactivate()
//    {
//        for (int i = 0; i < itemcount; i++)
//        {
//            scrollsnap.setitemstate(i, user.getinfo.userdata[i].bought);
//        }

//        updateitemstate(curactiveitem);


//        scrollsnap.snaptoobj(user.getinfo.curplayerindex, false);

//        if (!user.getinfo.allcharactersbought() /*&& user.havecoin (100)*/)
//            openboxbtn.gameobject.setactive(true);
//        else
//            openboxbtn.gameobject.setactive(false);

//        //init localizedprice
//        for (int i = 0; i < database.get.playersdata.length; i++)
//        {
//            database.get.playersdata[i].price = purchasemanager.ins.getlocalizedprice(database.get.playersdata[i].purchaseid);
//        }

//    }

//    public override void ondeactivate()
//    {
//        base.ondeactivate();
//    }

//    public override void oncleanup()
//    {
//        scrollsnap.onchangeitemevent -= onchangeitem;
//        purchasemanager.onpurchasenonconsumable -= buypaiditemsuccess;
//    }

//    void onchangeitem(int index)
//    {
//        itemnametext.text = localizationmanager.getlocalizedtext(database.get.playersdata[index].name);
//        itemabilitytext.text = localizationmanager.getlocalizedtext(database.get.playersdata[index].name + "_desc");

//        itembiomeinfotext.text = localizationmanager.getlocalizedtext("biomes") + ": " + biomecontroller.ins.getbiomesliststring(index);

//        updateitemstate(index);
//    }

//    public void selectandplay(int index)
//    {
//        user.setplayerindex(index);
//    }

//    /*	public void buyitemwithcoin (int index)
//	{
//		if (user.buywithcoin (database.get.playersdata [index].price)) {
//			user.getinfo.userdata [index].bought = true;
//			updateitemstate (index);
//			scrollsnap.setitemstate (index, user.getinfo.userdata [index].bought);
//			user.saveuserinfo ();
//		}
//	}*/

//    public void buypaiditem(int index)
//    {
//        purchasemanager.ins.buynonconsumable(index);
//    }

//    public void buypaiditemsuccess(purchaseeventargs args)
//    {
//        string purchid = args.purchasedproduct.definition.id;

//        int index = 0;

//        for (int i = 0; i < purchasemanager.ins.nc_products.length; i++)
//        {
//            if (purchid == purchasemanager.ins.nc_products[i])
//            {
//                index = i;
//                break;
//            }
//        }

//        debug.log("you bought " + purchid + "  id " + index + " noncon");
//        user.getinfo.userdata[index].bought = true;
//        updateitemstate(index);
//        scrollsnap.setitemstate(index, user.getinfo.userdata[index].bought);
//        user.saveuserinfo();
//    }

//    public void updateitemstate(int index)
//    {
//        bool bought = user.getinfo.userdata[index].bought;

//        selectandplaybtn.gameobject.setactive(bought);
//        buybtn.gameobject.setactive(!bought);
//        buybtn.getcomponentinchildren<text>().text = database.get.playersdata[index].price.tostring();
//    }

//    public void backbtn()
//    {
//        if (player.ins.isdead)
//        {
//            screencontroller.ins.activatescreen(screencontroller.gamescreen.gameover);
//        }
//        else
//            screencontroller.ins.activatescreen(screencontroller.gamescreen.menu);

//    }

//}
