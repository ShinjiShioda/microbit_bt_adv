input.onButtonPressed(Button.A, function () {
    閾値 += 8
    if (閾値 >= 255) {
        閾値 = 255
    }
    input.setSoundThreshold(SoundThreshold.Loud, 閾値)
    bluetooth.advertiseUid(
    name_space_threshold,
    閾値,
    7,
    false
    )
    basic.showArrow(ArrowNames.North)
    basic.showNumber(Math.round(Math.map(閾値, 0, 255, 0, 9)))
    basic.pause(BeaconCycle)
    basic.clearScreen()
    bluetooth.stopAdvertising()
})
input.onLogoEvent(TouchButtonEvent.Pressed, function () {
    閾値 = 128
    input.setSoundThreshold(SoundThreshold.Loud, 閾値)
    bluetooth.advertiseUid(
    name_space_threshold,
    閾値,
    7,
    false
    )
    basic.showArrow(ArrowNames.East)
    basic.showNumber(Math.round(Math.map(閾値, 0, 255, 0, 9)))
    basic.pause(BeaconCycle)
    basic.clearScreen()
    bluetooth.stopAdvertising()
})
input.onSound(DetectedSound.Loud, function () {
    音 = input.soundLevel()
    Event_Time = input.runningTime()
    basic.showNumber(Math.round(Math.map(音, 0, 255, 0, 9)))
})
input.onButtonPressed(Button.B, function () {
    閾値 += -8
    if (閾値 <= 0) {
        閾値 = 0
    }
    input.setSoundThreshold(SoundThreshold.Loud, 閾値)
    bluetooth.advertiseUid(
    name_space_threshold,
    閾値,
    7,
    false
    )
    basic.showArrow(ArrowNames.South)
    basic.showNumber(Math.round(Math.map(閾値, 0, 255, 0, 9)))
    basic.pause(BeaconCycle)
    basic.clearScreen()
    bluetooth.stopAdvertising()
})
let 音 = 0
let Event_Time = 0
let BeaconCycle = 0
let name_space_threshold = 0
let 閾値 = 0
bluetooth.setTransmitPower(7)
閾値 = 128
let Instance = 0
let name_space = 2155905152
name_space_threshold = 2155905153
BeaconCycle = 2000
Event_Time = input.runningTime()
input.setSoundThreshold(SoundThreshold.Loud, 閾値)
basic.showIcon(IconNames.Yes)
bluetooth.advertiseUid(
name_space,
0,
7,
false
)
basic.pause(BeaconCycle)
bluetooth.stopAdvertising()
basic.forever(function () {
    if (音 > 0) {
        bluetooth.advertiseUid(
        name_space,
        Event_Time,
        7,
        false
        )
        if (Event_Time + BeaconCycle < input.runningTime()) {
            音 = 0
            bluetooth.stopAdvertising()
            basic.clearScreen()
        }
    }
})
