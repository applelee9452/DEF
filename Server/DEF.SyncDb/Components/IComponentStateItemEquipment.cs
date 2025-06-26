namespace DEF.SyncDb;

// 装备类型
public enum EquipmentType
{
    None = 0,
    Weapon,// 武器
    Hat,// 帽子
    Mask,// 面饰
    Shoulder,// 护肩
    Coat,// 上衣
    Bracelet,// 臂甲
    Glove,// 手套
    Belt,// 腰带
    Kneepad,// 护膝
    Shoes,// 鞋子

    Mount,// 坐骑
    Artifact,// 神器
    Relic,// 遗物
    Back,// 翅膀
    Fly,// 飞宠
    Fate,// 武魂
    Ring,// 戒指
}

// 品质，共11种
public enum EquipmentQuality
{
    None = 0,
    Normal,// 普通
    Outstanding,// 优秀
    Excellent,// 精良
    Rare,// 稀有
    Prominence,// 卓越
    Epic,// 史诗
    Legend,// 传奇
    Immortal,// 不朽
    Transcend,// 超越
    Gilding,// 鎏金
    Eternal,// 永恒
}

// 战斗属性Key，用于装备动态词条
public enum PropKey
{
    None = 0,
    CritHit,// 角色高级属性，暴击，CritHit
    ComboHit,// 角色高级属性，连击，ComboHit
    CounterHit,// 角色高级属性，反击，CounterHit
    Stun,// 角色高级属性，击晕，Stun
    Dodge,// 角色高级属性，闪避，Dodge
    Heal,// 角色高级属性，回复，Heal
    IgnoreStun,// 角色高级属性，忽视击晕，IgnoreStun
    IgnoreDodge,// 角色高级属性，忽视闪避，IgnoreDodge
    IgnoreComboHit,// 角色高级属性，忽视连击，IgnoreComboHit
    IgnoreCounterHit,// 角色高级属性，忽视反击，IgnoreCounterHit
    CritDamage,// 角色高级属性，暴伤，CritDamage
    ResistDamage,// 角色高级属性，抗暴，ResistDamage
    AttackPower,// 角色高级属性，普攻系数，AttackPower
    AttackPowerReduce,// 角色高级属性，普攻减伤，AttackPowerReduce
    ComboHitPower,// 角色高级属性，连击系数，ComboHitPower
    ComboHitPowerReduce,// 角色高级属性，连击减伤，ComboHitPowerReduce
    CounterHitPower,// 角色高级属性，反击系数，CounterHitPower
    CounterHitPowerReduce,// 角色高级属性，反击减伤，CounterHitPowerReduce
    KnockUp,// 角色高级属性，击飞，KnockUp
    IgnoreKnockUp,// 角色高级属性，忽视击飞，IgnoreKnockUp
    SkillCritHit,// 角色高级属性，技能暴击，SkillCritHit
    SkillCritDamage,// 角色高级属性，技能暴伤，SkillCritDamage
    BossDamage,// 角色高级属性，Boss伤害，BossDamage
    BossDamageReduce,// 角色高级属性，Boss减伤，BossDamageReduce
    PartnerDamage,// 角色高级属性，同伴伤害，PartnerDamage
    PartnerDamageReduce,// 角色高级属性，同伴减伤，PartnerDamageReduce
    HealRate,// 角色高级属性，治愈率，HealRate
    HealAmount,// 角色高级属性，治愈量，HealAmount
    DamageReduce,// 角色高级属性，减伤，DamageReduce
    AttackSpeedBonus,// 角色高级属性，攻速加成，AttackSpeedBonus
    HpBonus,// 角色高级属性，生命加成，HpBonus
    AttackBonus,// 角色高级属性，攻击加成，AttackBonus
    DefBonus,// 角色高级属性，防御加成，DefBonus
    PartnerCritHit,// 角色高级属性，同伴暴击，DefBonus
    PartnerComboHit,// 角色高级属性，同伴连击，DefBonus
}

[Component("ItemEquipment")]
public partial interface IComponentStateItemEquipment : IComponentState
{
    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    EquipmentType EquipmentType { get; set; }// 装备类型

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    EquipmentQuality Quality { get; set; }// 装备品质

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int Lv { get; set; }// 等级

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int Hp { get; set; }// 生命，Hp

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int Attack { get; set; }// 攻击，Attack

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int Def { get; set; }// 防御，Def

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    float AttackSpeed { get; set; }// 攻速，AttackSpeed

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int AttrId1 { get; set; }// Attribute表id,战斗属性id

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    PropKey PropKey1 { get; set; }// 角色战斗属性Key，动态属性词条

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    float Value1 { get; set; }// 角色战斗属性Value，如2.72%，则Value1为0.0272

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int AttrId2 { get; set; }// Attribute表id,战斗属性id

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    PropKey PropKey2 { get; set; }// 角色战斗属性Key，动态属性词条

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    float Value2 { get; set; }// 角色战斗属性Value

}