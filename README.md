# VectorTileLayer
A Mapsui add-on to handle different vector tile and style formats.

This is work in progress and far from being perfect. Up to now, 
only [OpenMapTiles](https://openmaptiles.org/) formats could be 
handled. They are like MapboxGL formats, so it should be possible 
to read Mapbox GL formats too.

In the moment the symbols are rendered/layouted on the fly before 
drawing. This should go into an extra process, which is performed 
when the tiles are loaded. This should save a lot of time while 
drawing.