// Global window control helpers for Tauri
window.appWindowControls = {
    minimize: async () => {
        try {
            const win = window.__TAURI__.window.getCurrentWindow();
            await win.minimize();
        } catch (e) { console.error(e); }
    },
    toggleMaximize: async () => {
        try {
            const win = window.__TAURI__.window.getCurrentWindow();
            await win.toggleMaximize();
        } catch (e) { console.error(e); }
    },
    close: async () => {
        try {
            const win = window.__TAURI__.window.getCurrentWindow();
            await win.close();
        } catch (e) { console.error(e); }
    }
};