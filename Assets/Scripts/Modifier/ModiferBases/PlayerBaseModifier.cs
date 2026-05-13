public abstract class PlayerBaseModifier : BaseModifier
{
    protected Player player;

    public virtual void FindPlayer(Player playerToMod)
    {
        player = playerToMod;
    }
}
