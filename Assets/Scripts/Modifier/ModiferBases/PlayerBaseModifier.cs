public abstract class PlayerBaseModifier : BaseModifier
{
    protected Player player;

    public void FindPlayer(Player playerToMod)
    {
        player = playerToMod;
    }
}
