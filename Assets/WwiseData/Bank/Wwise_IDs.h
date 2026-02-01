/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID BACK = 1559875400U;
        static const AkUniqueID HIGHLIGHT = 3174665583U;
        static const AkUniqueID JUMP = 3833651337U;
        static const AkUniqueID PAUSEALL = 4091047182U;
        static const AkUniqueID PLAYERDAMAGE = 337406793U;
        static const AkUniqueID PLAYTITLE = 3593803035U;
        static const AkUniqueID POSSESS = 47246163U;
        static const AkUniqueID POSSESSNOT = 3348899946U;
        static const AkUniqueID PULLBACK = 830775945U;
        static const AkUniqueID RESUMEALL = 3240900869U;
        static const AkUniqueID SELECT = 1432588725U;
        static const AkUniqueID STARTGAME = 1521187885U;
        static const AkUniqueID STOPMUSIC = 1917263390U;
        static const AkUniqueID STOPPANIC = 2922217614U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace POSSESSED
        {
            static const AkUniqueID GROUP = 58477120U;

            namespace STATE
            {
                static const AkUniqueID JUSTMASK = 2665410631U;
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID POSSESSING = 3330989325U;
            } // namespace STATE
        } // namespace POSSESSED

    } // namespace STATES

    namespace SWITCHES
    {
        namespace GAMEPLAYMUSIC
        {
            static const AkUniqueID GROUP = 2275179450U;

            namespace SWITCH
            {
                static const AkUniqueID INGAME = 984691642U;
                static const AkUniqueID PREGAME = 3594052030U;
            } // namespace SWITCH
        } // namespace GAMEPLAYMUSIC

        namespace MUSIC
        {
            static const AkUniqueID GROUP = 3991942870U;

            namespace SWITCH
            {
                static const AkUniqueID GAMEPLAY = 89505537U;
                static const AkUniqueID TITLE = 3705726509U;
            } // namespace SWITCH
        } // namespace MUSIC

        namespace PULLBACKSWITCH
        {
            static const AkUniqueID GROUP = 75059705U;

            namespace SWITCH
            {
            } // namespace SWITCH
        } // namespace PULLBACKSWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID FXVOLUME = 3328840097U;
        static const AkUniqueID MASTERVOLUME = 2918011349U;
        static const AkUniqueID MUSICVOLUME = 2346531308U;
        static const AkUniqueID PULLPITCH = 1328605826U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID MAIN = 3161908922U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID MAIN_AUDIO_BUS = 2246998526U;
        static const AkUniqueID MUSIC = 3991942870U;
        static const AkUniqueID SFX = 393239870U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID HALLVERB = 2411154035U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
