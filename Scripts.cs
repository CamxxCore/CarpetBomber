using GTA.Native;
using GTA;

public static class Scripts
{
    public static void RequestScriptAudioBank(string name)
    {
        while (!Function.Call<bool>(Hash.REQUEST_SCRIPT_AUDIO_BANK, name, 0))
            Script.Wait(0);
    }

    public static void RequestPTFXAsset(string name)
    {
        if (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
        {
            Function.Call(Hash.REQUEST_NAMED_PTFX_ASSET, name);

            while (!Function.Call<bool>(Hash.HAS_NAMED_PTFX_ASSET_LOADED, name))
                Script.Wait(0);
        }
    }
}