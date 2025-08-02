
public enum NPCType
{ 
    Special,
    Generic
}

public enum Switch_Type
{
    Main,
    Blockers
}

public enum ObjectiveType
{
    Puzzle,
    MiniGame_BiasBingo,
    FightBots,
    ConvinceNPC,
    MiniGame_MalignInfluence,
    MiniGame_SpotTheSource,
    Generator,
    Trophy
}

public enum TypeOfSpeaker
{
    NPC,
    Player,
    Villain,
    Instructor
}

public enum NoticeType
{
    Hint,
    Exit,
    Wrong,
    Correct,
    Progress,
    QuestCompleted,
    ObjectiveCompleted
}

public enum MalignChecker
{
    True,
    False
}

public enum BiasChecker
{
    NegativityXCrisis_Bias,
    SensationalismXEmotionalism,
    False_Balance,
    Confirmation_Bias,
    GatekeepingXElite_Bias,
    Omission_Bias,
    FramingXSpin,
    StereotypingXMonolithic_Framing,
    Fear_Mongering,
    SelectionXAgenda_Setting_Bias
}

public enum SourceChecker
{
    //Health
    Health_Review,
    Health_Buzz_Daily,
    Global_Health_Journal,

    //Socials
    Society_Today,
    Viral_Trends_Daily,
    Community_Voices_Network,

    //Sports
    Athletic_Daily,
    Instant_Sports_Buzz,
    National_Sports_Review,

    //Government
    Political_Insight,
    Gov_Affairs_Weekly,
    Civic_Affairs_Tribune,

    //Finance
    Finance_Times,
    Quick_Stock_Tips,
    Market_Insight_Africa,

    //Tech
    Tech_Newsletter
}

public enum PatrolMode
{
    Idle,
    Walk,
    Interact
}

public enum SpeakerType
{
    Other,
    Player
}

public enum CharacterType
{
    NPC,
    Player,
    Villain
}

public enum PlayerResponseStyle
{
    LogicalTone = 0,      // Presents data or logic
    ReservedTone = 1,    // Cautious, measured warning
    EmotionalTone = 2,    // Leans on empathy or feeling
    ArgumentativeTone = 3,// Challenges their assumptions
}