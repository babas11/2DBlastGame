public interface IObstacle
{
    int Health { get; }
    void TakeDamage(int damage, bool isTNTBlast);

    void UpdateObjectives();

}
