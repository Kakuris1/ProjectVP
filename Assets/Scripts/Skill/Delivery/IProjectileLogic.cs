namespace Combat.Skills
{
    // 투사체 헬퍼 스크립트가 공통으로 가져야 할 '규칙'
    public interface IProjectileLogic
    {
        void Initialize(in SkillContext ctx);
    }
}