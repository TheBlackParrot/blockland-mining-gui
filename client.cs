$MiningClient::revision = 3;

function clientCmdMiningClient_receiveHandshake() {
	echo("CLIENT: RECEIVED HANDSHAKE");
	echo("CLIENT: SENT HANDSHAKE");
	commandToServer('MiningServer_receiveHandshake');
}

function clientCmdMiningClient_receiveVar(%type,%arg1,%arg2) {
	%global_formatting = "<font:Palatino Linotype Bold:16><just:right>";

	switch$(%type) {
		case "ore":
			%HUD_obj = %arg1 @ "count";
			if(isObject(%HUD_obj)) {
				%HUD_obj.setText(%global_formatting @ "<color:ffffff>" @ %arg2);
			}
		case "position":
			%x = mFloor(getWord(%arg1,0)) @ "x";
			%y = mFloor(getWord(%arg1,1)) @ "y";
			%z = mFloor(getWord(%arg1,2)) @ "z";
			PositionText.setText(%global_formatting @ "<color:ffffff>" @ %x SPC "<color:cccccc>" @ %y SPC "<color:999999>" @ %z);
		case "health":
			HPText.setText(%global_formatting @ "<color:ffffff>" @ %arg1 SPC "/" SPC %arg2);
		case "score":
			ScoreText.setText(%global_formatting @ "<color:ffffff>" @ %arg1);
	}
}

function MiningClient_dateLoop() {
	if($MiningClient::DateLoop) {
		cancel($MiningClient::DateLoop);
	}
	$MiningClient::DateLoop = schedule(1000,0,MiningClient_dateLoop);
	CurrentDateTimeText.setText("<font:Palatino Linotype Bold:16><color:00aaff><just:center>" @ getDateTime());
}

// christ, Blockland, please
function initMiningGUI() {
	exec("./gui.gui");
	MiningClient_dateLoop();
	commandToServer('MiningServer_requestGUIVars');
}

package MiningClientPackage {
	function PlayGui::onWake(%this) {
		parent::onWake(%this);
		schedule(100,0,initMiningGUI);
	}
	function PlayGui::onSleep(%this) {
		if(isObject(MiningHUD_Wrapper)) {
			MiningHUD_Wrapper.delete();
			cancel($MiningClient::DateLoop);
		}
		parent::onSleep(%this);
	}
};
activatePackage(MiningClientPackage);