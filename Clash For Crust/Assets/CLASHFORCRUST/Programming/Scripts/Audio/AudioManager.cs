using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioType { TitleScreen, Ready, CountDown, Chili, Donut, Firework, PopCorn, Shot, Smash, 
        Pause, UnPause, TimeTickin, Stun, DoubleStun, TripleStun, KnockOut, CarnivalSelect, BoardwalkSelect, Hightide,
    RedWins, GreenWins, BlueWins, YellowWins, AwardIntro, DropCrust, GetCrust, MatchOver, RulesMenu,
    PlayButton, OptionsButton, ExitGame, SeagullSlayer, FrenchConnection, PowerHungry, SeeingStars, GoldenGull, HotHeaded, Pyromaniac,
    TheKernel, Pyrotechnics, DrowningInDough, SpinToWin, SeagullSmasher, ManureMarksman, Schmoovin, Schmoovless, PityParty, Pacifist,
    BattleIntro, LowTide, PirateShipSelect, ScrewPickup, CustomizationMenu
    }

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                //Debug.LogError("Audio Manager is NULL");
            }

            return _instance;
        }
    }

    private AudioSource _audioSource;
    private AudioListener _listener;

    [SerializeField] float gracePeriod;

    [SerializeField] public bool clipPlaying; //Is on while a clip is playing
    [SerializeField] bool gracePeriodOn; //Is on when the grace period is active for non essential voice lines
    public bool pirateMode;

    [SerializeField] List<AudioClip> titleScreenClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> readyClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> countDownClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateCountDownClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> chiliClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateChiliClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> donutClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateDonutClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> fireworkClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateFireworkClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> popcornClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> piratePopcornClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> screwClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> shotClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateShotClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> smashClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateSmashClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pauseClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> piratePauseClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> unPauseClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateUnPauseClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> timesTickinClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateTimesTickinClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> stunClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateStunClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> doubleStunClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> tripleStunClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> knockOutClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateKnockOutClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> carnivalSelectClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> boardWalkSelectClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateShipSelectClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> highTideClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> lowTideClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> redWinsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> greenWinsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> blueWinsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> yellowWinsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> awardIntroClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> dropCrustClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateDropCrustClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> getCrustClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateGetCrustClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> matchOverClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateMatchOverClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> rulesMenuClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> playButtonClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> customizationMenuClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> optionsButtonClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> exitGameClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> seagullSlayerClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> frenchConnectionClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> powerHungryClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> seeingStarsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> goldenGullClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> hotHeadedClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pyromaniacClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> theKernelClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pyrotechnicsClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> drowningInDoughClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> spinToWinClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> seagullSmasherClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> manureMarksmanClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> schmoovinClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> schmoovlessClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pityPartyClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pacifistClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> carnyIntroClips = new List<AudioClip>();
    [SerializeField] List<AudioClip> pirateIntroClips = new List<AudioClip>();



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _listener = GetComponent<AudioListener>();
        _audioSource = GetComponent<AudioSource>();

        clipPlaying = false;
        gracePeriodOn = false;
        pirateMode = false;

    }

    private void Update()
    {
        IsAudioPlaying();
    }


    public void PlayAudio(AudioType audio, bool essential) //Takes in the type of audio that will be played and whether or not that clip is an essential clip.
    {
        List<AudioClip> clipList = new List<AudioClip>();

        //Big list of crap
        switch (audio)
        {
            case AudioType.TitleScreen:
                clipList = titleScreenClips;
                break;
            case AudioType.Ready:
                clipList = readyClips;
                break;
            case AudioType.CountDown:
                if (!pirateMode)
                    clipList = countDownClips;
                else
                    clipList = pirateCountDownClips;
                break;
            case AudioType.Chili:
                if (!pirateMode)
                    clipList = chiliClips;
                else
                    clipList = pirateChiliClips;
                break;
            case AudioType.Donut:
                if (!pirateMode)
                    clipList = donutClips;
                else
                    clipList = pirateDonutClips;
                break;
            case AudioType.Firework:
                if (!pirateMode)
                    clipList = fireworkClips;
                else
                    clipList = pirateFireworkClips;
                    break;
            case AudioType.PopCorn:
                if (!pirateMode)
                    clipList = popcornClips;
                else
                    clipList = piratePopcornClips;
                break;
            case AudioType.Shot:
                if (!pirateMode)
                    clipList = shotClips;
                else
                    clipList = pirateShotClips;
                break;
            case AudioType.Smash:
                if (!pirateMode)
                    clipList = smashClips;
                else
                    clipList = pirateSmashClips;
                break;
            case AudioType.Pause:
                if (!pirateMode)
                    clipList = pauseClips;
                else
                    clipList = piratePauseClips;
                break;
            case AudioType.UnPause:
                if (!pirateMode)
                    clipList = unPauseClips;
                else
                    clipList = pirateUnPauseClips;
                break;
            case AudioType.TimeTickin:
                if (!pirateMode)
                    clipList = timesTickinClips;
                else
                    clipList = pirateTimesTickinClips;
                break;
            case AudioType.Stun:
                clipList = stunClips;
                break;
            case AudioType.DoubleStun:
                clipList = doubleStunClips;
                break;
            case AudioType.TripleStun:
                clipList = tripleStunClips;
                break;
            case AudioType.KnockOut:
                if (!pirateMode)
                    clipList = knockOutClips;
                else
                    clipList = pirateKnockOutClips;
                break;
            case AudioType.CarnivalSelect:
                clipList = carnivalSelectClips;
                break;
            case AudioType.BoardwalkSelect:
                clipList = boardWalkSelectClips;
                break;
            case AudioType.Hightide:
                clipList = highTideClips;
                break;
            case AudioType.RedWins:
                clipList = redWinsClips;
                break;
            case AudioType.GreenWins:
                clipList = greenWinsClips;
                break;
            case AudioType.BlueWins:
                clipList = blueWinsClips;
                break;
            case AudioType.YellowWins:
                clipList = yellowWinsClips;
                break;
            case AudioType.AwardIntro:
                clipList = awardIntroClips;
                break;
            case AudioType.DropCrust:
                if (!pirateMode)
                    clipList = dropCrustClips;
                else
                    clipList = pirateDropCrustClips;
                break;
            case AudioType.GetCrust:
                if (!pirateMode)
                    clipList = getCrustClips;
                else
                    clipList = pirateGetCrustClips;
                break;
            case AudioType.MatchOver:
                if (!pirateMode)
                    clipList = matchOverClips;
                else
                    clipList = pirateMatchOverClips;
                break;
            case AudioType.RulesMenu:
                clipList = rulesMenuClips;
                break;
            case AudioType.PlayButton:
                clipList = playButtonClips;
                break;
            case AudioType.OptionsButton:
                clipList = optionsButtonClips;
                break;
            case AudioType.ExitGame:
                clipList = exitGameClips;
                break;
            case AudioType.SeagullSlayer:
                clipList = seagullSlayerClips;
                break;
            case AudioType.FrenchConnection:
                clipList = frenchConnectionClips;
                break;
            case AudioType.PowerHungry:
                clipList = powerHungryClips;
                break;
            case AudioType.SeeingStars:
                clipList = seeingStarsClips;
                break;
            case AudioType.GoldenGull:
                clipList = goldenGullClips;
                break;
            case AudioType.HotHeaded: 
                clipList = hotHeadedClips;
                break;
            case AudioType.Pyromaniac:
                clipList = pyromaniacClips;
                break;
            case AudioType.TheKernel:
                clipList = theKernelClips;
                break;
            case AudioType.Pyrotechnics:
                clipList = pyrotechnicsClips;
                break;
            case AudioType.DrowningInDough:
                clipList = drowningInDoughClips;
                break;
            case AudioType.SpinToWin: 
                clipList = spinToWinClips;
                break;
            case AudioType.SeagullSmasher:
                clipList = seagullSmasherClips;
                break;
            case AudioType.ManureMarksman:
                clipList = manureMarksmanClips;
                break;
            case AudioType.Schmoovin:
                clipList = schmoovinClips;
                break;
            case AudioType.Schmoovless:
                clipList = schmoovlessClips;
                break;
            case AudioType.PityParty:
                clipList = pityPartyClips;
                break;
            case AudioType.Pacifist:
                clipList = pacifistClips;
                break;
            case AudioType.BattleIntro:
                if (!pirateMode)
                    clipList = carnyIntroClips;
                else
                    clipList = pirateIntroClips;
                break;
            case AudioType.LowTide:
                clipList = lowTideClips;
                break;
            case AudioType.PirateShipSelect:
                clipList = pirateShipSelectClips;
                break;
            case AudioType.ScrewPickup:
                clipList = screwClips;
                break;
            case AudioType.CustomizationMenu:
                clipList = customizationMenuClips;
                break;

        }

        if (!clipPlaying && !gracePeriodOn) //If no clip is playing currently play a clip
        {
            int randNum = Random.Range(0, clipList.Count);
            _audioSource.clip = clipList[randNum];

            _audioSource.Play();
            StartGracePerod(gracePeriod);
        }

        if (essential) //If a clip is playing but the new clip is an essential clip, cut the current clip off and play the new one.
        {
            _audioSource.Stop();

            int randNum = Random.Range(0, clipList.Count);
            _audioSource.clip = clipList[randNum];

            _audioSource.Play();
            StartGracePerod(gracePeriod);
        }
        
    }

    public void StartCountdown(float delay, bool skipClash = false)
    {
        StartCoroutine(CountDown(delay));
    }

    private void StartGracePerod(float gracePeriod)
    {
        StartCoroutine(GracePeriod(gracePeriod));
    }
    
    private void IsAudioPlaying() //Runs in update checks to see if an announcer clip is playing
    {
        if (_audioSource.isPlaying)
            clipPlaying = true;
        else
            clipPlaying = false;
    }

    private IEnumerator GracePeriod(float gracePeriod)
    {
        gracePeriodOn = true;
        yield return new WaitForSeconds(gracePeriod);
        gracePeriodOn = false;
    }
    private IEnumerator CountDown(float delay, bool skipClash = false)
    {
        if (!pirateMode)
        {
            _audioSource.clip = countDownClips[2]; //3...
            _audioSource.Play();

            yield return new WaitForSecondsRealtime(delay);

            _audioSource.clip = countDownClips[1]; //2...
            _audioSource.Play();

            yield return new WaitForSecondsRealtime(delay);

            _audioSource.clip = countDownClips[0]; //1...
            _audioSource.Play();

            if (!skipClash)
            {
                yield return new WaitForSecondsRealtime(delay);

                _audioSource.clip = countDownClips[3]; //Clash!!!
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.clip = pirateCountDownClips[2]; //3...
            _audioSource.Play();

            yield return new WaitForSecondsRealtime(delay);

            _audioSource.clip = pirateCountDownClips[1]; //2...
            _audioSource.Play();

            yield return new WaitForSecondsRealtime(delay);

            _audioSource.clip = pirateCountDownClips[0]; //1...
            _audioSource.Play();

            if(!skipClash)
            {
                yield return new WaitForSecondsRealtime(delay);

                _audioSource.clip = pirateCountDownClips[3]; //Clash!!!
                _audioSource.Play();
            }
        }
    }

    public bool IsPlaying()
    {
        return _audioSource.isPlaying;
    }

    public void PirateModeToggle(bool on)
    {
        pirateMode = on;
    }
}