using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Facebook.Unity;

public class ctrProgressClass {
	static public Dictionary<string, int> progress = new Dictionary<string, int>();

	static public void saveProgress() {
		string strProgress = "";
		foreach (var item in progress ) {
			if (progressDefault.ContainsKey(item.Key)) if (progress[item.Key] != progressDefault[item.Key]) strProgress += item.Key + "=" + item.Value + ";";
		}
		PlayerPrefs.SetString("progress", strProgress);
		PlayerPrefs.Save();
	}

	static public void getProgress() {
		string strProgress = PlayerPrefs.GetString("progress");
		progress = new Dictionary<string, int> (progressDefault);

		string strKey = "", strValue = "";
		bool flag = true;
		for (int i = 0; i < strProgress.Length; i++) {
			if (strProgress.Substring(i, 1) == "=") flag = false;

			else if (strProgress.Substring(i, 1) == ";") {
				flag = true;
				progress[strKey] = int.Parse(strValue);
                //скины и шапки. запись в статик переменную
			    if (strKey == "skinCurrent") staticClass.currentSkin = "skin" + strValue;
                if (strKey == "berryCurrent") staticClass.currentBerry = "berry" + strValue;
                if (strKey == "hatCurrent") staticClass.currentHat = "hat" + strValue;
				strKey = "";
				strValue = "";
			} else if (flag) strKey += strProgress.Substring(i, 1);
			else if (!flag) strValue += strProgress.Substring(i, 1);

		}
	}


	static private Dictionary<string, int> progressDefault = new Dictionary<string, int>{
		{"googlePlay",0}, {"lastLevel",0}, {"currentLevel",1},{"coins",0},{"gems",0},{"energyTime",0},{"energy",4},{"energyInfinity",0},
        {"hints",0},{"webs",0},{"collectors",0},{"teleports",0},{"complect",0},{"music",1},{"sound",1},{"dailyBonus",0},{"language",0},
		{"tutorialBuy",0},{"everyplay",1},{"firstPurchase",0},{"fb",0},
        {"boostersWhite",0 }, {"boostersGreen",0 }, {"boostersBlue",0 }, {"boostersPurple",0 },
        {"berryRare", UnityEngine.Random.Range(2, 6)}, {"hatRare", UnityEngine.Random.Range(2, 6)},{"skinRare", UnityEngine.Random.Range(2, 6)}, 


        {"berry1",1},{"berry2",0},{"berry3",0},{"berry4",0},{"berry5",0},
		{"hat1",1},{"hat2",0},{"hat3",0},{"hat4",0},{"hat5",0},
		{"skin1",1},{"skin2",0},{"skin3",0},{"skin4",0},{"skin5",0},
        {"berryCurrent", 1}, {"hatCurrent", 1}, {"skinCurrent", 1},


        {"level1",0},{"level2",0},{"level3",0},{"level4",0},{"level5",0},{"level6",0},{"level7",0},{"level8",0},{"level9",0},{"level10",0},
		{"level11",0},{"level12",0},{"level13",0},{"level14",0},{"level15",0},{"level16",0},{"level17",0},{"level18",0},{"level19",0},{"level20",0},
		{"level21",0},{"level22",0},{"level23",0},{"level24",0},{"level25",0},{"level26",0},{"level27",0},{"level28",0},{"level29",0},{"level30",0},
		{"level31",0},{"level32",0},{"level33",0},{"level34",0},{"level35",0},{"level36",0},{"level37",0},{"level38",0},{"level39",0},{"level40",0},
		{"level41",0},{"level42",0},{"level43",0},{"level44",0},{"level45",0},{"level46",0},{"level47",0},{"level48",0},{"level49",0},{"level50",0},
		{"level51",0},{"level52",0},{"level53",0},{"level54",0},{"level55",0},{"level56",0},{"level57",0},{"level58",0},{"level59",0},{"level60",0},
		{"level61",0},{"level62",0},{"level63",0},{"level64",0},{"level65",0},{"level66",0},{"level67",0},{"level68",0},{"level69",0},{"level70",0},
		{"level71",0},{"level72",0},{"level73",0},{"level74",0},{"level75",0},{"level76",0},{"level77",0},{"level78",0},{"level79",0},{"level80",0},
		{"level81",0},{"level82",0},{"level83",0},{"level84",0},{"level85",0},{"level86",0},{"level87",0},{"level88",0},{"level89",0},{"level90",0},
		{"level91",0},{"level92",0},{"level93",0},{"level94",0},{"level95",0},{"level96",0},{"level97",0},{"level98",0},{"level99",0},{"level100",0},

		{"score1_1",0},{"score1_2",0},{"score2_1",0},{"score2_2",0},{"score3_1",0},{"score3_2",0},{"score4_1",0},{"score4_2",0},{"score5_1",0},{"score5_2",0},
		{"score6_1",0},{"score6_2",0},{"score7_1",0},{"score7_2",0},{"score8_1",0},{"score8_2",0},{"score9_1",0},{"score9_2",0},{"score10_1",0},{"score10_2",0},
		{"score11_1",0},{"score11_2",0},{"score12_1",0},{"score12_2",0},{"score13_1",0},{"score13_2",0},{"score14_1",0},{"score14_2",0},{"score15_1",0},{"score15_2",0},
		{"score16_1",0},{"score16_2",0},{"score17_1",0},{"score17_2",0},{"score18_1",0},{"score18_2",0},{"score19_1",0},{"score19_2",0},{"score20_1",0},{"score20_2",0},
		{"score21_1",0},{"score21_2",0},{"score22_1",0},{"score22_2",0},{"score23_1",0},{"score23_2",0},{"score24_1",0},{"score24_2",0},{"score25_1",0},{"score25_2",0},
		{"score26_1",0},{"score26_2",0},{"score27_1",0},{"score27_2",0},{"score28_1",0},{"score28_2",0},{"score29_1",0},{"score29_2",0},{"score30_1",0},{"score30_2",0},
		{"score31_1",0},{"score31_2",0},{"score32_1",0},{"score32_2",0},{"score33_1",0},{"score33_2",0},{"score34_1",0},{"score34_2",0},{"score35_1",0},{"score35_2",0},
		{"score36_1",0},{"score36_2",0},{"score37_1",0},{"score37_2",0},{"score38_1",0},{"score38_2",0},{"score39_1",0},{"score39_2",0},{"score40_1",0},{"score40_2",0},
		{"score41_1",0},{"score41_2",0},{"score42_1",0},{"score42_2",0},{"score43_1",0},{"score43_2",0},{"score44_1",0},{"score44_2",0},{"score45_1",0},{"score45_2",0},
		{"score46_1",0},{"score46_2",0},{"score47_1",0},{"score47_2",0},{"score48_1",0},{"score48_2",0},{"score49_1",0},{"score49_2",0},{"score50_1",0},{"score50_2",0},
		{"score51_1",0},{"score51_2",0},{"score52_1",0},{"score52_2",0},{"score53_1",0},{"score53_2",0},{"score54_1",0},{"score54_2",0},{"score55_1",0},{"score55_2",0},
		{"score56_1",0},{"score56_2",0},{"score57_1",0},{"score57_2",0},{"score58_1",0},{"score58_2",0},{"score59_1",0},{"score59_2",0},{"score60_1",0},{"score60_2",0},
		{"score61_1",0},{"score61_2",0},{"score62_1",0},{"score62_2",0},{"score63_1",0},{"score63_2",0},{"score64_1",0},{"score64_2",0},{"score65_1",0},{"score65_2",0},
		{"score66_1",0},{"score66_2",0},{"score67_1",0},{"score67_2",0},{"score68_1",0},{"score68_2",0},{"score69_1",0},{"score69_2",0},{"score70_1",0},{"score70_2",0},
		{"score71_1",0},{"score71_2",0},{"score72_1",0},{"score72_2",0},{"score73_1",0},{"score73_2",0},{"score74_1",0},{"score74_2",0},{"score75_1",0},{"score75_2",0},
		{"score76_1",0},{"score76_2",0},{"score77_1",0},{"score77_2",0},{"score78_1",0},{"score78_2",0},{"score79_1",0},{"score79_2",0},{"score80_1",0},{"score80_2",0},
		{"score81_1",0},{"score81_2",0},{"score82_1",0},{"score82_2",0},{"score83_1",0},{"score83_2",0},{"score84_1",0},{"score84_2",0},{"score85_1",0},{"score85_2",0},
		{"score86_1",0},{"score86_2",0},{"score87_1",0},{"score87_2",0},{"score88_1",0},{"score88_2",0},{"score89_1",0},{"score89_2",0},{"score90_1",0},{"score90_2",0},
		{"score91_1",0},{"score91_2",0},{"score92_1",0},{"score92_2",0},{"score93_1",0},{"score93_2",0},{"score94_1",0},{"score94_2",0},{"score95_1",0},{"score95_2",0},
		{"score96_1",0},{"score96_2",0},{"score97_1",0},{"score97_2",0},{"score98_1",0},{"score98_2",0},{"score99_1",0},{"score99_2",0},{"score100_1",0},{"score100_2",0},

		{"gift7",0},{"gift8",0},{"gift11",0},{"gift19",0},{"gift21",0},{"gift31",0},{"gift32",0},{"gift40",0},{"gift47",0},
		{"gift53",0},{"gift56",0},{"gift63",0},{"gift69",0},{"gift71",0},{"gift73",0},{"gift84",0},{"gift87",0},{"gift94",0},{"gift95",0}

	};
	static private Dictionary<string, int> progressCheat = new Dictionary<string, int>{
		{"googlePlay",0}, {"lastLevel",99}, {"currentLevel",1},{"coins",1000},{"gems",0},{"energyTime", 0},{"energy",4}, {"energyInfinity",0},
        {"hints",99},{"webs",99},{"collectors",99},{"teleports",99},{"complect",0},{"music",1},{"sound",1},{"dailyBonus",0},{"language",0},
		{"tutorialBuy",0},{"everyplay",1},{"firstPurchase",0},{"fb",0},

        {"boostersWhite",110 }, {"boostersGreen",220 }, {"boostersBlue",330 }, {"boostersPurple",110 },
        {"berryRare", 2 }, {"hatRare",2},{"skinRare", 4},


        {"berry1",1},{"berry2",2},{"berry3",1},{"berry4",5},{"berry5",1},
        {"hat1",1},{"hat2",1},{"hat3",1},{"hat4",4},{"hat5",0},
        {"skin1",1},{"skin2",50},{"skin3",2},{"skin4",12},{"skin5",1},
        {"berryCurrent", 1}, {"hatCurrent", 1}, {"skinCurrent", 1},

        {"level1",0},{"level2",0},{"level3",0},{"level4",0},{"level5",0},{"level6",0},{"level7",0},{"level8",0},{"level9",0},{"level10",0},
		{"level11",0},{"level12",0},{"level13",0},{"level14",0},{"level15",0},{"level16",0},{"level17",0},{"level18",0},{"level19",0},{"level20",0},
		{"level21",0},{"level22",0},{"level23",0},{"level24",0},{"level25",0},{"level26",0},{"level27",0},{"level28",0},{"level29",0},{"level30",0},
		{"level31",0},{"level32",0},{"level33",0},{"level34",0},{"level35",0},{"level36",0},{"level37",0},{"level38",0},{"level39",0},{"level40",0},
		{"level41",0},{"level42",0},{"level43",0},{"level44",0},{"level45",0},{"level46",0},{"level47",0},{"level48",0},{"level49",0},{"level50",0},
		{"level51",0},{"level52",0},{"level53",0},{"level54",0},{"level55",0},{"level56",0},{"level57",0},{"level58",0},{"level59",0},{"level60",0},
		{"level61",0},{"level62",0},{"level63",0},{"level64",0},{"level65",0},{"level66",0},{"level67",0},{"level68",0},{"level69",0},{"level70",0},
		{"level71",0},{"level72",0},{"level73",0},{"level74",0},{"level75",0},{"level76",0},{"level77",0},{"level78",0},{"level79",0},{"level80",0},
		{"level81",0},{"level82",0},{"level83",0},{"level84",0},{"level85",0},{"level86",0},{"level87",0},{"level88",0},{"level89",0},{"level90",0},
		{"level91",0},{"level92",0},{"level93",0},{"level94",0},{"level95",0},{"level96",0},{"level97",0},{"level98",0},{"level99",0},{"level100",0},

		{"score1_1",0},{"score1_2",0},{"score2_1",0},{"score2_2",0},{"score3_1",0},{"score3_2",0},{"score4_1",0},{"score4_2",0},{"score5_1",0},{"score5_2",0},
		{"score6_1",0},{"score6_2",0},{"score7_1",0},{"score7_2",0},{"score8_1",0},{"score8_2",0},{"score9_1",0},{"score9_2",0},{"score10_1",0},{"score10_2",0},
		{"score11_1",0},{"score11_2",0},{"score12_1",0},{"score12_2",0},{"score13_1",0},{"score13_2",0},{"score14_1",0},{"score14_2",0},{"score15_1",0},{"score15_2",0},
		{"score16_1",0},{"score16_2",0},{"score17_1",0},{"score17_2",0},{"score18_1",0},{"score18_2",0},{"score19_1",0},{"score19_2",0},{"score20_1",0},{"score20_2",0},
		{"score21_1",0},{"score21_2",0},{"score22_1",0},{"score22_2",0},{"score23_1",0},{"score23_2",0},{"score24_1",0},{"score24_2",0},{"score25_1",0},{"score25_2",0},
		{"score26_1",0},{"score26_2",0},{"score27_1",0},{"score27_2",0},{"score28_1",0},{"score28_2",0},{"score29_1",0},{"score29_2",0},{"score30_1",0},{"score30_2",0},
		{"score31_1",0},{"score31_2",0},{"score32_1",0},{"score32_2",0},{"score33_1",0},{"score33_2",0},{"score34_1",0},{"score34_2",0},{"score35_1",0},{"score35_2",0},
		{"score36_1",0},{"score36_2",0},{"score37_1",0},{"score37_2",0},{"score38_1",0},{"score38_2",0},{"score39_1",0},{"score39_2",0},{"score40_1",0},{"score40_2",0},
		{"score41_1",0},{"score41_2",0},{"score42_1",0},{"score42_2",0},{"score43_1",0},{"score43_2",0},{"score44_1",0},{"score44_2",0},{"score45_1",0},{"score45_2",0},
		{"score46_1",0},{"score46_2",0},{"score47_1",0},{"score47_2",0},{"score48_1",0},{"score48_2",0},{"score49_1",0},{"score49_2",0},{"score50_1",0},{"score50_2",0},
		{"score51_1",0},{"score51_2",0},{"score52_1",0},{"score52_2",0},{"score53_1",0},{"score53_2",0},{"score54_1",0},{"score54_2",0},{"score55_1",0},{"score55_2",0},
		{"score56_1",0},{"score56_2",0},{"score57_1",0},{"score57_2",0},{"score58_1",0},{"score58_2",0},{"score59_1",0},{"score59_2",0},{"score60_1",0},{"score60_2",0},
		{"score61_1",0},{"score61_2",0},{"score62_1",0},{"score62_2",0},{"score63_1",0},{"score63_2",0},{"score64_1",0},{"score64_2",0},{"score65_1",0},{"score65_2",0},
		{"score66_1",0},{"score66_2",0},{"score67_1",0},{"score67_2",0},{"score68_1",0},{"score68_2",0},{"score69_1",0},{"score69_2",0},{"score70_1",0},{"score70_2",0},
		{"score71_1",0},{"score71_2",0},{"score72_1",0},{"score72_2",0},{"score73_1",0},{"score73_2",0},{"score74_1",0},{"score74_2",0},{"score75_1",0},{"score75_2",0},
		{"score76_1",0},{"score76_2",0},{"score77_1",0},{"score77_2",0},{"score78_1",0},{"score78_2",0},{"score79_1",0},{"score79_2",0},{"score80_1",0},{"score80_2",0},
		{"score81_1",0},{"score81_2",0},{"score82_1",0},{"score82_2",0},{"score83_1",0},{"score83_2",0},{"score84_1",0},{"score84_2",0},{"score85_1",0},{"score85_2",0},
		{"score86_1",0},{"score86_2",0},{"score87_1",0},{"score87_2",0},{"score88_1",0},{"score88_2",0},{"score89_1",0},{"score89_2",0},{"score90_1",0},{"score90_2",0},
		{"score91_1",0},{"score91_2",0},{"score92_1",0},{"score92_2",0},{"score93_1",0},{"score93_2",0},{"score94_1",0},{"score94_2",0},{"score95_1",0},{"score95_2",0},
		{"score96_1",0},{"score96_2",0},{"score97_1",0},{"score97_2",0},{"score98_1",0},{"score98_2",0},{"score99_1",0},{"score99_2",0},{"score100_1",0},{"score100_2",0},

		{"gift7",0},{"gift8",0},{"gift11",0},{"gift19",0},{"gift21",0},{"gift31",0},{"gift32",0},{"gift40",0},{"gift47",0},
		{"gift53",0},{"gift56",0},{"gift63",0},{"gift69",0},{"gift71",0},{"gift73",0},{"gift84",0},{"gift87",0},{"gift94",0},{"gift95",0}

	};
	static public void resetProgress(string nameButton) {
		//сброс прогресса
		progress = new Dictionary<string, int> (progressDefault);
		if (nameButton == "reset cheat") progress = new Dictionary<string, int> (progressCheat);
		saveProgress ();
		//PlayerPrefs.SetString("progress", staticClass.strProgressDefault);

		//ctrProgressClass.getProgress();
		//GooglePlayManager.instance.ResetAllAchievements();
		//GooglePlayManager.instance.SubmitScore("leaderboard_test_leaderboard", 0);
	}

}

