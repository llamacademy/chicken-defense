namespace LlamAcademy.ChickenDefense.Units
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
        void OnMouseIn();
        void OnMouseOut();
    }
}