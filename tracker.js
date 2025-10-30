// Portfolio Analytics Tracker
// Add this script to your portfolio website

(function() {
    'use strict';
    
    const config = {
        apiUrl: 'http://localhost:5000/api/analytics/track', // Update with your API URL
        sessionDuration: 30 * 60 * 1000, // 30 minutes
    };

    // Get or create session ID
    function getSessionId() {
        let sessionId = sessionStorage.getItem('analytics_session_id');
        const sessionTime = sessionStorage.getItem('analytics_session_time');
        const now = Date.now();

        if (!sessionId || !sessionTime || (now - parseInt(sessionTime)) > config.sessionDuration) {
            sessionId = crypto.randomUUID();
            sessionStorage.setItem('analytics_session_id', sessionId);
        }
        
        sessionStorage.setItem('analytics_session_time', now.toString());
        return sessionId;
    }

    // Detect device type
    function getDeviceType() {
        const ua = navigator.userAgent;
        if (/(tablet|ipad|playbook|silk)|(android(?!.*mobi))/i.test(ua)) {
            return 'tablet';
        }
        if (/Mobile|Android|iP(hone|od)|IEMobile|BlackBerry|Kindle|Silk-Accelerated|(hpw|web)OS|Opera M(obi|ini)/.test(ua)) {
            return 'mobile';
        }
        return 'desktop';
    }

    // Detect browser and OS
    function getBrowserInfo() {
        const ua = navigator.userAgent;
        let browser = 'Unknown';
        let os = 'Unknown';

        // Browser detection
        if (ua.includes('Firefox')) browser = 'Firefox';
        else if (ua.includes('Chrome') && !ua.includes('Edg')) browser = 'Chrome';
        else if (ua.includes('Safari') && !ua.includes('Chrome')) browser = 'Safari';
        else if (ua.includes('Edg')) browser = 'Edge';
        else if (ua.includes('Opera') || ua.includes('OPR')) browser = 'Opera';

        // OS detection
        if (ua.includes('Win')) os = 'Windows';
        else if (ua.includes('Mac')) os = 'macOS';
        else if (ua.includes('Linux')) os = 'Linux';
        else if (ua.includes('Android')) os = 'Android';
        else if (ua.includes('iOS')) os = 'iOS';

        return { browser, os };
    }

    // Extract UTM parameters
    function getUtmParams() {
        const params = new URLSearchParams(window.location.search);
        return {
            utmSource: params.get('utm_source'),
            utmMedium: params.get('utm_medium'),
            utmCampaign: params.get('utm_campaign'),
        };
    }

    // Track event
    async function trackEvent(eventType, customData = {}) {
        const { browser, os } = getBrowserInfo();
        const utmParams = getUtmParams();
        
        const data = {
            eventType,
            pageUrl: window.location.href,
            pageTitle: document.title,
            referrer: document.referrer || null,
            sessionId: getSessionId(),
            browser,
            os,
            device: getDeviceType(),
            screenResolution: `${screen.width}x${screen.height}`,
            viewportWidth: window.innerWidth,
            viewportHeight: window.innerHeight,
            ...utmParams,
            ...customData,
        };

        try {
            await fetch(config.apiUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data),
            });
        } catch (error) {
            console.error('Analytics tracking error:', error);
        }
    }

    // Track page view
    function trackPageView() {
        const loadTime = performance.timing 
            ? performance.timing.loadEventEnd - performance.timing.navigationStart 
            : null;
        
        trackEvent('pageview', { loadTime });
    }

    // Track page visibility changes
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
            trackEvent('page_hidden');
        } else {
            trackEvent('page_visible');
        }
    });

    // Track scroll depth
    let maxScroll = 0;
    window.addEventListener('scroll', () => {
        const scrollPercent = Math.round((window.scrollY / (document.documentElement.scrollHeight - window.innerHeight)) * 100);
        if (scrollPercent > maxScroll) {
            maxScroll = scrollPercent;
            if (scrollPercent === 25 || scrollPercent === 50 || scrollPercent === 75 || scrollPercent === 100) {
                trackEvent('scroll', { customData: { depth: scrollPercent } });
            }
        }
    });

    // Track page view on load
    if (document.readyState === 'complete') {
        trackPageView();
    } else {
        window.addEventListener('load', trackPageView);
    }

    // Expose tracking function globally for custom events
    window.portfolioAnalytics = {
        track: trackEvent,
    };
})();
