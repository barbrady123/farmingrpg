public enum ToolEffect
{
    None = 0,

    Watering = 1
}

public enum Direction
{
    Up = 0,

    Down = 1,

    Left = 2,

    Right = 3,

    None = 4
}

public enum ItemType
{
    Seed = 0,

    Commodity = 1,

    WateringTool = 2,

    HoeingTool = 3,

    ChoppingTool = 4,

    BreakingTool = 5,

    ReapingTool = 6,

    CollectingTool = 7,

    ReapableScenery = 8,

    Furniture = 9,

    None = 10
}

public enum InventoryLocation
{
    Player = 0,

    Chest = 1
}

public enum AnimationName
{
    IdleUp = 0,

    IdleDown = 1,

    IdleRight = 2,

    IdleLeft = 3,

    WalkUp = 4,

    WalkDown = 5,

    WalkRight = 6,

    WalkLeft = 7,

    RunUp = 8,

    RunDown = 9,

    RunRight = 10,

    RunLeft = 11,

    UseToolUp = 12,

    UseToolDown = 13,

    UseToolRight = 14,

    UseToolLeft = 15,

    SwingToolUp = 16,

    SwingToolDown = 17,

    SwingToolRight = 18,

    SwingToolLeft = 19,

    LiftToolUp = 20,

    LiftToolDown = 21,

    LiftToolRight = 22,

    LiftToolLeft = 23,

    HoldToolUp = 24,

    HoldToolDown = 25,

    HoldToolRight = 26,

    HoldToolLeft = 27,

    PickUp = 28,

    PickDown = 29,

    PickRight = 30,

    PickLeft = 31
}

public enum CharacterPartAnimator
{
    Body = 0,

    Arms = 1,

    Hair = 2,

    Tool = 3,

    Hat = 4
}

public enum PartVariantColor
{
    None = 0
}

public enum PartVariantType
{
    None = 0,

    Carry = 1,

    Hoe = 2,

    Pickaxe = 3,

    Axe = 4,

    Scythe = 5,

    WateringCan = 6
}

public enum Season
{
    Spring = 0,

    Summer = 1,

    Autumn = 2,

    Winter = 3
}

public enum SceneName
{
    Scene1_Farm = 0,

    Scene2_Field = 1,

    Scene3_Cabin = 2
}

public enum GridBoolProperty
{
    Diggable = 0,

    CanDropItem = 1,

    CanPlaceFurniture = 2,

    IsPath = 3,

    IsNPCObstacle = 4
}

public enum HarvestActionEffect
{
    DeciduousLeavesFalling = 0,

    PineConesFalling = 1,

    ChoppingTreeTrunk = 2,

    BreakingStone = 3,

    Reaping = 4,

    None = 5
}

public enum Facing
{
    None = 0,

    Front = 1,

    Back = 2,

    Right = 3
}