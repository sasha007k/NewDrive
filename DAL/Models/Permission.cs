namespace DAL.Models
{
    public enum PermissionType
    {
        View,
        Edit
    }

    public class Permission
    {
        public int Id { get; set; }

        public PermissionType PermissionType { get; set; }
    }
}
