import { observer } from "mobx-react-lite";
import { useStore } from "../stores/rootStore";
import { Code } from "@heroui/code";
export const VersionStatus = observer(() => {
  const rootStore = useStore()


  return <>
    <div className="flex flex-wrap gap-4">
      <Code color="primary">{rootStore.statusStore.isOnline && "v" + rootStore.statusStore.versionInfo?.version}</Code>
    </div>
  </>
})
