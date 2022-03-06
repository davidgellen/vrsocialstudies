using Dp;
using UnityEngine;

public class AvatarSelection : MonoBehaviour
{
    public Terminal terminal;
    public float RotationSpeed = 10f;

    private int _currentAvatar = 0;
    private int _nextAvatar = 0;
    private GameObject[] _avatars;
    private bool _avatarSelected = false;

    private void Start()
    {
        terminal.gameObject.SetActive(false);
        _avatars = new GameObject[AvatarManager.Instance.AvatarModels.Length];

        int i = 0;
        foreach (var item in AvatarManager.Instance.AvatarModels)
        {
            var av = Instantiate(item, transform);
            av.SetActive(false);
            _avatars[i] = av;
            i++;
        }

        ShowAvatar();
    }

    private void Update()
    {
        if (_avatars.Length == 0)
            return;

        _avatars[_currentAvatar].transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed);
    }

    public void SelectAvatar() 
    {
        if (!_avatarSelected)
        {           
            terminal.gameObject.SetActive(true);
            AvatarManager.Instance.SelectedAvatar = _nextAvatar;
            AvatarManager.Instance.AvatarSelected(terminal);
            _avatarSelected = true;
        }
        else 
        {
            AvatarManager.Instance.LeapMotionLikeGestureDetected();
            _avatarSelected = false;
        }
    }

    public void GetPreviousAvatar()
    {
        if (_avatars.Length == 0)
            return;

        _nextAvatar = (--_nextAvatar) < 0 ? _avatars.Length - 1 : _nextAvatar;
        ShowAvatar();
    }

    public void GetNextAvatar() 
    {
        if (_avatars.Length == 0)
            return;

        _nextAvatar = (++_nextAvatar) >= _avatars.Length ? 0 : _nextAvatar;      
        ShowAvatar();
    }

    void ShowAvatar() 
    {
        _avatars[_currentAvatar].SetActive(false);
        _avatars[_nextAvatar].SetActive(true);
        _currentAvatar = _nextAvatar;       
    }
}
