//
//  ChartboostManager.m
//  CB
//
//  Created by Mike DeSaro on 12/20/11.
//

#import "ChartboostManager.h"


void UnitySendMessage( const char * className, const char * methodName, const char * param );

void UnityPause( bool pause );


@implementation ChartboostManager

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (ChartboostManager*)sharedManager
{
	static ChartboostManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[ChartboostManager alloc] init];
	
	return sharedSingleton;
}


+ (id)objectFromJson:(NSString*)json
{
	NSError *error = nil;
	NSData *data = [NSData dataWithBytes:json.UTF8String length:json.length];
    NSObject *object = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingAllowFragments error:&error];
	
	if( error )
		NSLog( @"failed to deserialize JSON: %@ with error: %@", json, [error localizedDescription] );
    
    return object;
}


+ (NSString*)jsonFromObject:(id)object
{
	NSError *error = nil;
	NSData *jsonData = [NSJSONSerialization dataWithJSONObject:object options:0 error:&error];
	
	if( jsonData && !error )
		return [[[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding] autorelease];
	else
		NSLog( @"jsonData was null, error: %@", [error localizedDescription] );
    
    return @"{}";
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)startChartBoostWithAppId:(NSString*)appId appSignature:(NSString*)appSignature
{
    Chartboost *cb = [Chartboost sharedChartboost];
    cb.appId = appId;
    cb.appSignature = appSignature;
    cb.delegate = self;
    
    [cb startSession];
}


- (void)cacheInterstitial:(NSString*)location
{
    if( location )
        [[Chartboost sharedChartboost] cacheInterstitial:location];
    else
        [[Chartboost sharedChartboost] cacheInterstitial];
}


- (void)showInterstitial:(NSString*)location
{
    if( location )
        [[Chartboost sharedChartboost] showInterstitial:location];
    else
        [[Chartboost sharedChartboost] showInterstitial];
}


- (void)cacheMoreApps
{
    [[Chartboost sharedChartboost] cacheMoreApps];
}


- (void)showMoreApps
{
    [[Chartboost sharedChartboost] showMoreApps];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark ChartboostDelegate

- (BOOL)shouldDisplayInterstitial:(NSString*)location
{
    UnityPause( true );
    return YES;
}


// Called when an interstitial has failed to come back from the server
- (void)didFailToLoadInterstitial:(NSString*)location
{
    UnitySendMessage( "ChartboostManager", "didFailToLoadInterstitial", location.UTF8String );
}


- (void)didCacheInterstitial:(NSString*)location
{
	UnitySendMessage( "ChartboostManager", "didCacheInterstitial", location.UTF8String );
}


// Called when the user dismisses the interstitial
- (void)didDismissInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didDismissInterstitial", location.UTF8String );
}


// Same as above, but only called when dismissed for a close
- (void)didCloseInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didCloseInterstitial", location.UTF8String );
}


// Same as above, but only called when dismissed for a click
- (void)didClickInterstitial:(NSString*)location
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didClickInterstitial", location.UTF8String );
}


// Called when a more apps page has failed to come back from the server
- (void)didFailToLoadMoreApps
{
    UnitySendMessage( "ChartboostManager", "didFailToLoadMoreApps", "" );
}


- (void)didCacheMoreApps
{
	UnitySendMessage( "ChartboostManager", "didCacheMoreApps", "" );
}


- (BOOL)shouldDisplayMoreApps
{
    UnityPause( true );
    return YES;
}


- (void)didDismissMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didFinishMoreApps", "dismiss" );
}


// Same as above, but only called when dismissed for a close
- (void)didCloseMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didFinishMoreApps", "close" );
}


// Same as above, but only called when dismissed for a click
- (void)didClickMoreApps
{
    UnityPause( false );
    UnitySendMessage( "ChartboostManager", "didFinishMoreApps", "click" );
}


- (BOOL)shouldRequestInterstitialsInFirstSession
{
	return _shouldRequestInterstitialsInFirstSession;
}

@end
