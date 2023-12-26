# HookUI

> Library for plugin authors to easily manage plugin localizations.

LanguageManager is a tool library for plugin authors to manage localizations of their plugins UI.

# Requirements

- BepInEx Unity Mono [Version 5](https://github.com/BepInEx/BepInEx/releases) or [Version 6](https://builds.bepinex.dev/projects/bepinex_be)

# Documentation for plugin authors

## Reference

Add reference of "LanguageManagerLib.dll" to your plugin project.

## Register your plugin

Use LanguageEntryManager.Register(mycoolpluginID, defaultLanguageCode)  in yor Plugin.Awake() method to register you plugin to LanguageManager.

## Language files 

Add language files in "Languages" directory, witch looks like:
```
MyCoolPlugin
│  MyCoolPlugin.dll
│
└─Languages
        mycoolpluginID.en-US.json
        mycoolpluginID.zh-HANS.json
        ...
```
"mycoolpluginID" is an unique id of your plugin, you can name it your self but keep the same;

A language file content would like this:
```
{
    "key1": "hello",
    "key2": "world"
    ...
}
```

## In UI

Use react to load string the same as data.

If you are modding with Hook UI, you can do like this:
```
const [myString, setMyString] = react.useState('');

useDataUpdate(react, 'mycoolpluginID.key1', setMyString);
```

If not, do like this:
```
const [myString, setMyString] = react.useState('');

react.useEffect(() => {
    const event = "mycoolpluginID.key1"
    const updateEvent = event + ".update"
    const subscribeEvent = event + ".subscribe"
    const unsubscribeEvent = event + ".unsubscribe"

    var sub = engine.on(updateEvent, (data) => {
        setMyString(data)
    })
    engine.trigger(subscribeEvent)
    return () => {
        engine.trigger(unsubscribeEvent)
        sub.clear();
    };
}, [])
```
