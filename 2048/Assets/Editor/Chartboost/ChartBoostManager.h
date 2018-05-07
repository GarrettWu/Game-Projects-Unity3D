//
//  ChartBoostManager.h
//  CB
//
//  Created by Mike DeSaro on 12/20/11.
//


#import <Foundation/Foundation.h>
#import "Chartboost.h"



@interface ChartboostManager : NSObject <ChartboostDelegate>
@property (nonatomic) BOOL shouldRequestInterstitialsInFirstSession;


+ (ChartboostManager*)sharedManager;

+ (id)objectFromJson:(NSString*)json;

+ (NSString*)jsonFromObject:(id)object;


- (void)startChartBoostWithAppId:(NSString*)appId appSignature:(NSString*)appSignature;

- (void)cacheInterstitial:(NSString*)location;

- (void)showInterstitial:(NSString*)location;

- (void)cacheMoreApps;

- (void)showMoreApps;

@end
