const rankingRows = document.getElementById('ranking-rows');

async function refreshRanking() {
    try {
        if (!document.hidden) {
            const response = await fetch(rankingRows.dataset.refreshUrl, { signal: AbortSignal.timeout(5000) });
            if (response.ok) {
                rankingRows.innerHTML = await response.text();
            }
        }
    } catch {
        // Keep the displayed rankings and retry on the next interval.
    } finally {
        window.setTimeout(refreshRanking, 5000);
    }
}

window.setTimeout(refreshRanking, 5000);
